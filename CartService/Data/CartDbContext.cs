using Microsoft.EntityFrameworkCore;
using CartService.Data.Models;

namespace CartService.Data
{
    public class CartDbContext(DbContextOptions<CartDbContext> options) : DbContext(options)
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CartDbContext).Assembly);
        }
    }
}

