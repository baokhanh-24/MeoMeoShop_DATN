using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.ToTable("Material");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(50).HasColumnType("nvarchar(50)");
            builder.Property(x => x.Description).HasMaxLength(500).HasColumnType("nvarchar(500)");
        }
    }
}
