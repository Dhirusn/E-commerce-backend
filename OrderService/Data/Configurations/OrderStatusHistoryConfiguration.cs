using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Data.Models;

namespace OrderService.Data.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.ConfigureBaseEntity<OrderStatusHistory, Guid>();
            
            builder.Property(osh => osh.Status)
                   .IsRequired()
                   .HasConversion<string>();
            
            builder.Property(osh => osh.ChangedBy)
                   .HasMaxLength(450);
            
            builder.Property(osh => osh.Notes)
                   .HasMaxLength(1000);

            builder.HasOne(osh => osh.Order)
                   .WithMany(o => o.StatusHistory)
                   .HasForeignKey(osh => osh.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(osh => osh.OrderId);
            builder.HasIndex(osh => osh.Status);
            builder.HasIndex(osh => osh.ChangedAt);
        }
    }
}
