using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations;

public class OrderDetailInventoryBatchConfiguration : IEntityTypeConfiguration<OrderDetailInventoryBatch>
{
    public void Configure(EntityTypeBuilder<OrderDetailInventoryBatch> builder)
    {
        builder.ToTable("OrderDetailInventoryBatches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasOne(x => x.OrderDetail)
            .WithMany(od => od.OrderDetailInventoryBatches)
            .HasForeignKey(x => x.OrderDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InventoryBatch)
            .WithMany(ib => ib.OrderDetailInventoryBatches)
            .HasForeignKey(x => x.InventoryBatchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}