﻿using Bookshop.DataAccess.Data;
using Bookshop.DataAccess.Repository.IRepository;
using Bookshop.Models;
using Bookshop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Bookshop.Areas.Admin.Controllers
{
	[Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
			
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
			IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});

			ProductVM productVM = new()
			{
				CategoryList = CategoryList,
				Product = new Product()
			};

			if (id == null || id == 0)
			{
				// Create
				return View(productVM);
			}
			else
			{	
				// Update
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}
			
        }
        [HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
            if (ModelState.IsValid)
            {
				string wwwRootPath = _webHostEnvironment.WebRootPath;

				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl)) 
					{ 
						// Delete old image first
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				if (productVM.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
					TempData["success"] = "Product created successfully";
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
					TempData["success"] = "Product updated successfully";
				}

				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});
				return View(productVM);
			}
				
		}
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);

			if (ProductFromDb == null)
			{
				return NotFound();
			}

			return View(ProductFromDb);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id)
		{
			Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

			if (obj == null)
			{
				return NotFound();
			}

			_unitOfWork.Product.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = "Product deleted successfully";
			return RedirectToAction("Index");
		}
	}
}
