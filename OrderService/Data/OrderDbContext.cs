using Microsoft.EntityFrameworkCore;
using OrderService.Data.Models;
using OrderService.Data.Configurations;

namespace OrderService.Data
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
            
            // Configure owned types for addresses
            AddressConfiguration.ConfigureShippingAddress(modelBuilder);
            AddressConfiguration.ConfigureBillingAddress(modelBuilder);
        }
    }
}
