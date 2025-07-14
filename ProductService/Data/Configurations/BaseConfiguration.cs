using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.Common;

namespace ProductService.Data.Configurations
{
    public static class EntityConfigurationExtensions
    {
        public static void ConfigureBaseEntity<T, TId>(this EntityTypeBuilder<T> builder)
            where T : BaseEntity<TId>
        {
            builder.HasKey(e => e.Id);

            if (typeof(TId) == typeof(Guid))
            {
                builder.Property(nameof(BaseEntity<TId>.Id))
                       .HasColumnName("Id")
                       .HasDefaultValueSql("NEWSEQUENTIALID()");
            }
            else if (typeof(TId) == typeof(int))
            {
                builder.Property(nameof(BaseEntity<TId>.Id))
                       .HasColumnName("Id")
                       .ValueGeneratedOnAdd();
            }

            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.IsDeleted)
                   .HasDefaultValue(false);
        }
    }


}
