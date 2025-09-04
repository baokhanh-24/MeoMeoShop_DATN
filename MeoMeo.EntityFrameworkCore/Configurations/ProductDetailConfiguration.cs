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
            builder.Property(x => x.Sku).HasMaxLength(50).HasColumnType("varchar(50)");
            
            // Cấu hình các thuộc tính vận chuyển
            builder.Property(x => x.Weight).HasDefaultValue(500); // Mặc định 500g
            builder.Property(x => x.Length).HasDefaultValue(15);  // Mặc định 15cm
            builder.Property(x => x.Width).HasDefaultValue(15);   // Mặc định 15cm
            builder.Property(x => x.Height).HasDefaultValue(15);  // Mặc định 15cm
            
            // Cấu hình giới hạn mua hàng
            builder.Property(x => x.MaxBuyPerOrder).IsRequired(false); // Có thể null
            
            // Foreign keys
            builder.HasOne(x => x.Product).WithMany(c => c.ProductDetails).HasForeignKey(x => x.ProductId);
            builder.HasOne(x => x.Size).WithMany(c => c.ProductDetails).HasForeignKey(x => x.SizeId);
            builder.HasOne(x => x.Colour).WithMany(c => c.ProductDetails).HasForeignKey(x => x.ColourId);
            
            // Unique constraint
            builder.HasIndex(x => new { x.ProductId, x.SizeId, x.ColourId }).IsUnique();
        }
    }
}
