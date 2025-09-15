using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class InventoryBatchConfiguration : IEntityTypeConfiguration<InventoryBatch>
    {
        public void Configure(EntityTypeBuilder<InventoryBatch> builder)
        {
            builder.ToTable("InventoryBatches");
            builder.HasKey(x => x.Id);

            // Foreign key relationships
            builder.HasOne(x => x.ImportBatch)
                   .WithMany(x => x.InventoryBatches)
                   .HasForeignKey(x => x.ImportBatchId)
                   .OnDelete(DeleteBehavior.Cascade).IsRequired(false);

            builder.HasOne(x => x.ProductDetail)
                   .WithMany(x => x.InventoryBatches)
                   .HasForeignKey(x => x.ProductDetailId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
