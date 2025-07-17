using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Data.Models;

namespace PaymentService.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            // Configure base entity properties
            builder.ConfigureBaseEntity<Payment, Guid>();

            // Table name
            builder.ToTable("Payments");

            // Properties
            builder.Property(p => p.OrderId)
                   .IsRequired();

            builder.Property(p => p.UserId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                   .IsRequired()
                   .HasMaxLength(3)
                   .HasDefaultValue("USD");

            builder.Property(p => p.PaymentMethod)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(p => p.Status)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasDefaultValue(PaymentStatus.Pending);

            builder.Property(p => p.PaymentIntentId)
                   .HasMaxLength(100);

            builder.Property(p => p.TransactionId)
                   .HasMaxLength(100);

            builder.Property(p => p.PaymentGateway)
                   .HasMaxLength(50)
                   .HasDefaultValue("Stripe");

            builder.Property(p => p.ProcessedAt)
                   .IsRequired(false);

            builder.Property(p => p.FailureReason)
                   .HasMaxLength(500);

            builder.Property(p => p.PaymentMetadata)
                   .HasColumnType("nvarchar(max)");

            // Relationships
            builder.HasMany(p => p.Transactions)
                   .WithOne(t => t.Payment)
                   .HasForeignKey(t => t.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Refunds)
                   .WithOne(r => r.Payment)
                   .HasForeignKey(r => r.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.OrderId)
                   .HasDatabaseName("IX_Payments_OrderId");

            builder.HasIndex(p => p.UserId)
                   .HasDatabaseName("IX_Payments_UserId");

            builder.HasIndex(p => p.Status)
                   .HasDatabaseName("IX_Payments_Status");

            builder.HasIndex(p => p.PaymentMethod)
                   .HasDatabaseName("IX_Payments_PaymentMethod");

            builder.HasIndex(p => p.PaymentIntentId)
                   .HasDatabaseName("IX_Payments_PaymentIntentId")
                   .IsUnique()
                   .HasFilter("[PaymentIntentId] IS NOT NULL");

            builder.HasIndex(p => p.TransactionId)
                   .HasDatabaseName("IX_Payments_TransactionId")
                   .IsUnique()
                   .HasFilter("[TransactionId] IS NOT NULL");

            builder.HasIndex(p => p.CreatedAt)
                   .HasDatabaseName("IX_Payments_CreatedAt");

            builder.HasIndex(p => p.ProcessedAt)
                   .HasDatabaseName("IX_Payments_ProcessedAt");
        }
    }
}
