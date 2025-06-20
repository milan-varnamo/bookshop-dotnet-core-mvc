using bookshop_razor_temp.Data;
using bookshop_razor_temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace bookshop_razor_temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
		public List<Category> CategoryList { get; set; }
		public IndexModel(ApplicationDbContext db)
		{
            _db = db;
		}
		public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
        }
    }
}
