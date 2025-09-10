using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EmployeeName).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.Code)
                .HasMaxLength(20);
            builder.HasIndex(x => x.Code)
                .IsUnique();
            builder.Property(x => x.CustomerName).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.EmployeePhoneNumber).HasMaxLength(12).HasColumnType("varchar(12)");
            builder.Property(x => x.CustomerPhoneNumber).HasMaxLength(12).HasColumnType("varchar(12)");
            builder.Property(x => x.EmployeeEmail).HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.CustomerEmail).HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.DeliveryAddress).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.Property(x => x.Note).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.Property(x => x.Reason).HasMaxLength(500).HasColumnType("nvarchar(500)");
            builder.HasOne(x => x.Customers).WithMany(x => x.Orders).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Employee).WithMany(x => x.Orders).HasForeignKey(x => x.EmployeeId).IsRequired(false);
            builder.HasOne(x => x.Voucher).WithMany(x => x.Orders).HasForeignKey(x => x.VoucherId).IsRequired(false);
        }
    }
}
