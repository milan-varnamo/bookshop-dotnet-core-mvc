using bookshop_dotnet_core_mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace bookshop_dotnet_core_mvc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
    }
}
