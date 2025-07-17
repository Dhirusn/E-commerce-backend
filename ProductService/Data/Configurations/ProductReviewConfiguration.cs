using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Data.Models;

namespace ProductService.Data.Configurations
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            // Configure base entity properties
            builder.ConfigureBaseEntity<ProductReview, Guid>();

            // Table name
            builder.ToTable("ProductReviews");

            // Properties
            builder.Property(pr => pr.ProductId)
                   .IsRequired();

            builder.Property(pr => pr.UserId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(pr => pr.Rating)
                   .IsRequired()
                   .HasDefaultValue(1);

            builder.Property(pr => pr.Comment)
                   .HasMaxLength(2000);

            // Relationships
            builder.HasOne(pr => pr.Product)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(pr => pr.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pr => pr.ProductId)
                   .HasDatabaseName("IX_ProductReviews_ProductId");

            builder.HasIndex(pr => pr.UserId)
                   .HasDatabaseName("IX_ProductReviews_UserId");

            builder.HasIndex(pr => pr.Rating)
                   .HasDatabaseName("IX_ProductReviews_Rating");

            builder.HasIndex(pr => pr.CreatedAt)
                   .HasDatabaseName("IX_ProductReviews_CreatedAt");

            // Unique constraint to prevent duplicate reviews from same user for same product
            builder.HasIndex(pr => new { pr.ProductId, pr.UserId })
                   .IsUnique()
                   .HasDatabaseName("IX_ProductReviews_ProductId_UserId");

            // Check constraint for rating range
            builder.HasCheckConstraint("CK_ProductReviews_Rating", "[Rating] >= 1 AND [Rating] <= 5");
        }
    }
}
