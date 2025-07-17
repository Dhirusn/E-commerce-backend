using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Data.Models;

namespace OrderService.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ConfigureBaseEntity<OrderItem, Guid>();
            
            builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
            
            builder.Property(oi => oi.ProductName)
                   .IsRequired()
                   .HasMaxLength(255);
            
            builder.Property(oi => oi.ProductSku)
                   .IsRequired()
                   .HasMaxLength(100);
            
            builder.Property(oi => oi.ProductImageUrl)
                   .HasMaxLength(512);
            
            builder.Property(oi => oi.ProductAttributes)
                   .HasMaxLength(1000);

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(oi => oi.OrderId);
            builder.HasIndex(oi => oi.ProductId);
        }
    }
}
