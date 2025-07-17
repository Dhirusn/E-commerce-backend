using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Data.Models;

namespace OrderService.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ConfigureBaseEntity<Order, Guid>();
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.ShippingAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);

            builder.HasMany(o => o.StatusHistory)
                   .WithOne(h => h.Order)
                   .HasForeignKey(h => h.OrderId);
        }
    }
}
