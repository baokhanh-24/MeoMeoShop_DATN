using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Images");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(1000).HasColumnType("nvarchar(1000)");
            builder.Property(x => x.URL).HasMaxLength(1000).HasColumnType("varchar(1000)");
            builder.HasOne(x => x.Product).WithMany(x => x.Images).HasForeignKey(x => x.ProductId);
        }
    }
}
