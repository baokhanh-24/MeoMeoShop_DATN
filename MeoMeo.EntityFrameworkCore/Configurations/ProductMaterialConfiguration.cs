using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductMaterialConfiguration : IEntityTypeConfiguration<ProductMaterial>
    {
        public void Configure(EntityTypeBuilder<ProductMaterial> builder)
        {
            builder.ToTable("ProductMaterials");
            builder.HasKey(x => new
            {
                x.ProductId,
                x.MaterialId
            });
            builder.HasOne(x => x.Product).WithMany(x => x.ProductMaterials).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.Material).WithMany(x => x.ProductMaterials).HasForeignKey(x => x.MaterialId);
        }
    }
}
