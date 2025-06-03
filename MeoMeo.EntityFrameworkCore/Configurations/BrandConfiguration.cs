using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brand");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.Property(x => x.Code).HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(x => x.Country).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.Description).HasMaxLength(500).HasColumnType("nvarchar(500)");
            builder.Property(x => x.Description).HasMaxLength(500).HasColumnType("varchar(500)");
        }
    }
}
