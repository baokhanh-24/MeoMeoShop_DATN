using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class CommuneConfiguration : IEntityTypeConfiguration<Commune>
    {
        public void Configure(EntityTypeBuilder<Commune> builder)
        {
            builder.ToTable("Commune");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(50).HasColumnType("nvarchar(50)");
            builder.Property(x => x.Code).HasMaxLength(10).HasColumnType("varchar(10)");
            builder.HasOne(x => x.District).WithMany(x => x.Communes).HasForeignKey(x => x.DistrictId);
        }
    }
}
