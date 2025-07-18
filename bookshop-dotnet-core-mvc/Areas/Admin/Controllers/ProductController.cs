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
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			
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
					TempData["success"] = "Product was created successfully";
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
					TempData["success"] = "Product was updated successfully";
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

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			
			return Json(new {data = objProductList});
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();
			
			return Json(new { success = true, message = "Product was deleted successfully" });
		}

		#endregion
	}
}
