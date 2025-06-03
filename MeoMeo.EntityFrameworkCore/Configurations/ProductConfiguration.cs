using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.Property(x => x.Thumbnail).HasMaxLength(100).HasColumnType("varchar(500)");
            builder.HasOne(x => x.Brand).WithMany(x => x.Products).HasForeignKey(x => x.BrandId);
        }

    }
}
