using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class DistrictConfiguration : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable("Districts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(50).HasColumnType("nvarchar(50)");
            builder.Property(x => x.Code).HasMaxLength(10).HasColumnType("varchar(10)");
            builder.HasOne(x => x.Province).WithMany(x => x.Districts).HasForeignKey(x => x.ProvinceId);
        }
    }
}
