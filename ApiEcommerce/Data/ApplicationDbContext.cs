using ApiEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace ApiEcommerce.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):IdentityDbContext<ApplicationUser>(options) // Solución al constructor base
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Crucial para que Identity configure sus tablas
        }

        // Tus tablas personalizadas de negocio
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        //public DbSet<User> Users { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
