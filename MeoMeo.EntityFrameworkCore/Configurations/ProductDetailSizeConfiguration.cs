using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductDetailSizeConfiguration : IEntityTypeConfiguration<ProductDetailSize>
    {
        public void Configure(EntityTypeBuilder<ProductDetailSize> builder)
        {
            builder.ToTable("ProductDetailSize");
            builder.HasKey(x => x.ProductDetailId);
            builder.HasKey(x => x.SizeId);
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.ProductDetailSizes).HasForeignKey(x => x.ProductDetailId);
            builder.HasOne(x => x.Size).WithMany(x => x.ProductDetailSizes).HasForeignKey(x => x.SizeId);
        }
    }
}
