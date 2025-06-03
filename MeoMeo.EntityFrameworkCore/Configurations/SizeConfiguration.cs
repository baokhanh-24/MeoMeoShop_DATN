using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.ToTable("Sizes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Value).HasMaxLength(50).HasColumnType("nvarchar(50)");
            builder.Property(x => x.Code).HasMaxLength(20).HasColumnType("varchar(20)");
        }
    }
}
