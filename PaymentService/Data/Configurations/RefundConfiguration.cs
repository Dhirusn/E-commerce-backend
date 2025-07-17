using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Data.Models;

namespace PaymentService.Data.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            // Configure base entity properties
            builder.ConfigureBaseEntity<Refund, Guid>();

            // Table name
            builder.ToTable("Refunds");

            // Properties
            builder.Property(r => r.PaymentId)
                   .IsRequired();

            builder.Property(r => r.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(r => r.Status)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasDefaultValue(RefundStatus.Pending);

            builder.Property(r => r.RefundId)
                   .HasMaxLength(100);

            builder.Property(r => r.GatewayResponse)
                   .HasColumnType("nvarchar(max)");

            builder.Property(r => r.ProcessedAt)
                   .IsRequired(false);

            builder.Property(r => r.FailureReason)
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(r => r.Payment)
                   .WithMany(p => p.Refunds)
                   .HasForeignKey(r => r.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => r.PaymentId)
                   .HasDatabaseName("IX_Refunds_PaymentId");

            builder.HasIndex(r => r.RefundId)
                   .HasDatabaseName("IX_Refunds_RefundId")
                   .IsUnique()
                   .HasFilter("[RefundId] IS NOT NULL");

            builder.HasIndex(r => r.Status)
                   .HasDatabaseName("IX_Refunds_Status");

            builder.HasIndex(r => r.CreatedAt)
                   .HasDatabaseName("IX_Refunds_CreatedAt");

            builder.HasIndex(r => r.ProcessedAt)
                   .HasDatabaseName("IX_Refunds_ProcessedAt");
        }
    }
}
