using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class CustomersConfiguration : IEntityTypeConfiguration<Customers>
    {
        public void Configure(EntityTypeBuilder<Customers> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.Code).HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(x => x.PhoneNumber).HasMaxLength(12).HasColumnType("varchar(12)");
            builder.Property(x => x.TaxCode).HasMaxLength(14).HasColumnType("varchar(14)");
            builder.Property(x => x.Address).HasMaxLength(500).HasColumnType("nvarchar(500)");
            builder.HasOne(x => x.User).WithOne(x => x.Customers).HasForeignKey<Customers>(x => x.UserId);
        }
    }
}
