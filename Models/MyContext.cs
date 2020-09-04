using Microsoft.EntityFrameworkCore;
using productsAndCategories.Models;

namespace productsAndCategories.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter alongcopy
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Association> Associations { get; set; }
    }
}