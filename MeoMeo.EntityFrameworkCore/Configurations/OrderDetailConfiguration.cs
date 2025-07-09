using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Sku).HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(x => x.ProductName).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.Image).HasMaxLength(500).HasColumnType("nvarchar(1000)");
            builder.HasOne(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderId);
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.OrderDetails).HasForeignKey(x => x.ProductDetailId);
            builder.HasOne(x => x.PromotionDetail).WithMany(x => x.OrderDetails).HasForeignKey(x => x.PromotionDetailId);
            builder.HasOne(x => x.InventoryBatch).WithMany(x => x.OrderDetails).HasForeignKey(x => x.InventoryBatchId).OnDelete(DeleteBehavior.NoAction).IsRequired(false);
        }
    }
}
