using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Data.Models;

namespace OrderService.Data.Configurations
{
    public class AddressConfiguration
    {
        public static void ConfigureShippingAddress(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Order>()
                .OwnsOne(o => o.ShippingAddress);

            builder.Property(sa => sa.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(sa => sa.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(sa => sa.AddressLine1)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(sa => sa.AddressLine2)
                   .HasMaxLength(200);

            builder.Property(sa => sa.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(sa => sa.State)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(sa => sa.PostalCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(sa => sa.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(sa => sa.Phone)
                   .HasMaxLength(20);

            // Configure column names
            builder.Property(sa => sa.FirstName)
                   .HasColumnName("ShippingAddress_FirstName");

            builder.Property(sa => sa.LastName)
                   .HasColumnName("ShippingAddress_LastName");

            builder.Property(sa => sa.AddressLine1)
                   .HasColumnName("ShippingAddress_AddressLine1");

            builder.Property(sa => sa.AddressLine2)
                   .HasColumnName("ShippingAddress_AddressLine2");

            builder.Property(sa => sa.City)
                   .HasColumnName("ShippingAddress_City");

            builder.Property(sa => sa.State)
                   .HasColumnName("ShippingAddress_State");

            builder.Property(sa => sa.PostalCode)
                   .HasColumnName("ShippingAddress_PostalCode");

            builder.Property(sa => sa.Country)
                   .HasColumnName("ShippingAddress_Country");

            builder.Property(sa => sa.Phone)
                   .HasColumnName("ShippingAddress_Phone");
        }

        public static void ConfigureBillingAddress(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Order>()
                .OwnsOne(o => o.BillingAddress);

            builder.Property(ba => ba.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ba => ba.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ba => ba.AddressLine1)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(ba => ba.AddressLine2)
                   .HasMaxLength(200);

            builder.Property(ba => ba.City)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ba => ba.State)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ba => ba.PostalCode)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(ba => ba.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ba => ba.Phone)
                   .HasMaxLength(20);

            // Configure column names
            builder.Property(ba => ba.FirstName)
                   .HasColumnName("BillingAddress_FirstName");

            builder.Property(ba => ba.LastName)
                   .HasColumnName("BillingAddress_LastName");

            builder.Property(ba => ba.AddressLine1)
                   .HasColumnName("BillingAddress_AddressLine1");

            builder.Property(ba => ba.AddressLine2)
                   .HasColumnName("BillingAddress_AddressLine2");

            builder.Property(ba => ba.City)
                   .HasColumnName("BillingAddress_City");

            builder.Property(ba => ba.State)
                   .HasColumnName("BillingAddress_State");

            builder.Property(ba => ba.PostalCode)
                   .HasColumnName("BillingAddress_PostalCode");

            builder.Property(ba => ba.Country)
                   .HasColumnName("BillingAddress_Country");

            builder.Property(ba => ba.Phone)
                   .HasColumnName("BillingAddress_Phone");
        }
    }
}
