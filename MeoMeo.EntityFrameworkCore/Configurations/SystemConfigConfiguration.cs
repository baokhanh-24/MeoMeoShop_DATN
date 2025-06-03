using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
    {
        public void Configure(EntityTypeBuilder<SystemConfig> builder)
        {
            builder.ToTable("SystemConfig");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.Value).HasMaxLength(1000).HasColumnType("nvarchar(1000)");
        }
    }
}
