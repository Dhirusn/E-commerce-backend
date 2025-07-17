using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartService.Data.Models;

namespace CartService.Data.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            // Configure base entity properties
            builder.ConfigureBaseEntity<CartItem, Guid>();

            // Table name
            builder.ToTable("CartItems");

            // Properties
            builder.Property(ci => ci.CartId)
                   .IsRequired();

            builder.Property(ci => ci.ProductId)
                   .IsRequired();

            builder.Property(ci => ci.ProductName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(ci => ci.ProductSku)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ci => ci.UnitPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(ci => ci.Quantity)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(ci => ci.TotalPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(ci => ci.ProductImageUrl)
                   .HasMaxLength(500);

            builder.Property(ci => ci.ProductAttributes)
                   .HasMaxLength(1000);

            builder.Property(ci => ci.AvailableStock)
                   .IsRequired(false);

            builder.Property(ci => ci.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            // Relationships
            builder.HasOne(ci => ci.Cart)
                   .WithMany(c => c.CartItems)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ci => ci.CartId)
                   .HasDatabaseName("IX_CartItems_CartId");

            builder.HasIndex(ci => ci.ProductId)
                   .HasDatabaseName("IX_CartItems_ProductId");

            builder.HasIndex(ci => ci.ProductSku)
                   .HasDatabaseName("IX_CartItems_ProductSku");

            builder.HasIndex(ci => ci.IsAvailable)
                   .HasDatabaseName("IX_CartItems_IsAvailable");

            // Unique constraint to prevent duplicate products in the same cart
            builder.HasIndex(ci => new { ci.CartId, ci.ProductId, ci.ProductAttributes })
                   .IsUnique()
                   .HasDatabaseName("IX_CartItems_CartId_ProductId_Attributes");
        }
    }
}
