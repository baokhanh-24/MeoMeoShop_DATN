using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class InventoryBatchConfiguration : IEntityTypeConfiguration<InventoryBatch>
    {
        public void Configure(EntityTypeBuilder<InventoryBatch> builder)
        {
            builder.ToTable("InventoryBatch");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(x => x.Note).HasMaxLength(500).HasColumnType("nvarchar(500)");
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.InventoryBatches).HasForeignKey(x => x.ProductDetailId);
        }
    }
}
