using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Data.Models;

namespace ProductService.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ConfigureBaseEntity<Product, Guid>();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
