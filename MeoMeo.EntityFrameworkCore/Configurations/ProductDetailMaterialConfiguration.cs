using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductDetailMaterialConfiguration : IEntityTypeConfiguration<ProductDetailMaterial>
    {
        public void Configure(EntityTypeBuilder<ProductDetailMaterial> builder)
        {
            builder.ToTable("ProductDetailMaterial");
            builder.HasKey(x => x.ProductDetailId);
            builder.HasKey(x => x.MaterialId);
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.ProductDetailMaterials).HasForeignKey(x => x.ProductDetailId);
            builder.HasOne(x => x.Material).WithMany(x => x.ProductDetailMaterials).HasForeignKey(x => x.MaterialId);
        }
    }
}
