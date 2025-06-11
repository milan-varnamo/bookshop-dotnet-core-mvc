using bookshop_dotnet_core_mvc.Data;
using bookshop_dotnet_core_mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace bookshop_dotnet_core_mvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

    }
}
