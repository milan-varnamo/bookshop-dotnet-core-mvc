using Microsoft.AspNetCore.Mvc;

namespace bookshop_dotnet_core_mvc.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
