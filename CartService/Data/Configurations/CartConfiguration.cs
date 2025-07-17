using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CartService.Data.Models;

namespace CartService.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            // Configure base entity properties
            builder.ConfigureBaseEntity<Cart, Guid>();

            // Table name
            builder.ToTable("Carts");

            // Properties
            builder.Property(c => c.UserId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.UserEmail)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(c => c.Status)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(c => c.TotalAmount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            builder.Property(c => c.TotalItems)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(c => c.Notes)
                   .HasMaxLength(500);

            builder.Property(c => c.ExpiresAt)
                   .IsRequired(false);

            builder.Property(c => c.CouponCode)
                   .HasMaxLength(50);

            builder.Property(c => c.DiscountAmount)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0);

            // Relationships
            builder.HasMany(c => c.CartItems)
                   .WithOne(ci => ci.Cart)
                   .HasForeignKey(ci => ci.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.UserId)
                   .HasDatabaseName("IX_Carts_UserId");

            builder.HasIndex(c => c.UserEmail)
                   .HasDatabaseName("IX_Carts_UserEmail");

            builder.HasIndex(c => c.Status)
                   .HasDatabaseName("IX_Carts_Status");

            builder.HasIndex(c => c.CreatedAt)
                   .HasDatabaseName("IX_Carts_CreatedAt");

            builder.HasIndex(c => c.ExpiresAt)
                   .HasDatabaseName("IX_Carts_ExpiresAt");
        }
    }
}
