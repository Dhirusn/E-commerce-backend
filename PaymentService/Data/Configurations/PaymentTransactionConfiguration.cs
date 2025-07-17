using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Data.Models;

namespace PaymentService.Data.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            // Configure base entity properties
            builder.ConfigureBaseEntity<PaymentTransaction, Guid>();

            // Table name
            builder.ToTable("PaymentTransactions");

            // Properties
            builder.Property(pt => pt.PaymentId)
                   .IsRequired();

            builder.Property(pt => pt.TransactionId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(pt => pt.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(pt => pt.Currency)
                   .IsRequired()
                   .HasMaxLength(3)
                   .HasDefaultValue("USD");

            builder.Property(pt => pt.Type)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(pt => pt.Status)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasDefaultValue(TransactionStatus.Pending);

            builder.Property(pt => pt.GatewayResponse)
                   .HasColumnType("nvarchar(max)");

            builder.Property(pt => pt.ProcessedAt)
                   .IsRequired(false);

            builder.Property(pt => pt.FailureReason)
                   .HasMaxLength(500);

            // Relationships
            builder.HasOne(pt => pt.Payment)
                   .WithMany(p => p.Transactions)
                   .HasForeignKey(pt => pt.PaymentId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pt => pt.PaymentId)
                   .HasDatabaseName("IX_PaymentTransactions_PaymentId");

            builder.HasIndex(pt => pt.TransactionId)
                   .HasDatabaseName("IX_PaymentTransactions_TransactionId")
                   .IsUnique();

            builder.HasIndex(pt => pt.Type)
                   .HasDatabaseName("IX_PaymentTransactions_Type");

            builder.HasIndex(pt => pt.Status)
                   .HasDatabaseName("IX_PaymentTransactions_Status");

            builder.HasIndex(pt => pt.CreatedAt)
                   .HasDatabaseName("IX_PaymentTransactions_CreatedAt");

            builder.HasIndex(pt => pt.ProcessedAt)
                   .HasDatabaseName("IX_PaymentTransactions_ProcessedAt");
        }
    }
}
