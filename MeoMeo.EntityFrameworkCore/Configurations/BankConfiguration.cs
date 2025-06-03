using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Bank");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.Property(x => x.Logo).HasMaxLength(500).HasColumnType("varchar(500)");
        }
    }
}
