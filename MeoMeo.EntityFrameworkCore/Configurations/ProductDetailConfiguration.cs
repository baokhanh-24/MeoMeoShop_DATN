using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
    {
        public void Configure(EntityTypeBuilder<ProductDetail> builder)
        {
            builder.ToTable("ProductDetails");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Sku).HasMaxLength(20).HasColumnType("varchar(20)");
            builder.Property(x => x.Description).HasColumnType("nvarchar(max)");
            builder.HasOne(x => x.Product).WithMany(c => c.ProductDetails).HasForeignKey(x => x.ProductId);
        }
    }
}
