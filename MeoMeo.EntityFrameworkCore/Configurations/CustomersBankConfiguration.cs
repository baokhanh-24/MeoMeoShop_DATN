using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class CustomersBankConfiguration : IEntityTypeConfiguration<CustomersBank>
    {
        public void Configure(EntityTypeBuilder<CustomersBank> builder)
        {
            builder.ToTable("CustomersBank");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AccountNumber).HasMaxLength(15).HasColumnType("varchar(15)");
            builder.Property(x => x.Beneficiary).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.HasOne(x => x.Customers).WithMany(x => x.CustomersBanks).HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Bank).WithMany(x => x.CustomersBanks).HasForeignKey(x => x.BankId);
        }
    }
}
