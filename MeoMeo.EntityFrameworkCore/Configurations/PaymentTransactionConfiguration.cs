using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Basic Properties
            builder.Property(x => x.TransactionCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.VnpTransactionNo)
                .HasMaxLength(50);

            builder.Property(x => x.VnpTxnRef)
                .HasMaxLength(50);

            builder.Property(x => x.VnpOrderInfo)
                .HasMaxLength(500);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // VNPay Properties
            builder.Property(x => x.VnpBankCode)
                .HasMaxLength(20);

            builder.Property(x => x.VnpBankTranNo)
                .HasMaxLength(50);

            builder.Property(x => x.VnpCardType)
                .HasMaxLength(20);

            builder.Property(x => x.VnpPayDate)
                .HasMaxLength(14);

            builder.Property(x => x.VnpResponseCode)
                .HasMaxLength(10);

            builder.Property(x => x.VnpTransactionStatus)
                .HasMaxLength(10);

            builder.Property(x => x.ResponseMessage)
                .HasMaxLength(500);

            builder.Property(x => x.VnpSecureHash)
                .HasMaxLength(256);

            // Additional Properties
            builder.Property(x => x.IpAddress)
                .HasMaxLength(45); // IPv6 support

            builder.Property(x => x.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("VND");

            builder.Property(x => x.Locale)
                .HasMaxLength(5)
                .HasDefaultValue("vn");

            builder.Property(x => x.ErrorCode)
                .HasMaxLength(20);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(500);

            // Raw Data (can be large)
            builder.Property(x => x.RawRequestData)
                .HasColumnType("text");

            builder.Property(x => x.RawResponseData)
                .HasColumnType("text");

            // Default Values
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(x => x.IsValidated)
                .HasDefaultValue(false);

            builder.Property(x => x.RetryCount)
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(x => x.TransactionCode)
                .IsUnique()
                .HasDatabaseName("IX_PaymentTransactions_TransactionCode");

            builder.HasIndex(x => x.OrderId)
                .HasDatabaseName("IX_PaymentTransactions_OrderId");

            builder.HasIndex(x => x.CustomerId)
                .HasDatabaseName("IX_PaymentTransactions_CustomerId");

            builder.HasIndex(x => x.VnpTransactionNo)
                .HasDatabaseName("IX_PaymentTransactions_VnpTransactionNo");

            builder.HasIndex(x => x.Status)
                .HasDatabaseName("IX_PaymentTransactions_Status");

            builder.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("IX_PaymentTransactions_CreatedAt");

            builder.HasIndex(x => new { x.Status, x.CreatedAt })
                .HasDatabaseName("IX_PaymentTransactions_Status_CreatedAt");

            // Foreign Key Relationships
            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
