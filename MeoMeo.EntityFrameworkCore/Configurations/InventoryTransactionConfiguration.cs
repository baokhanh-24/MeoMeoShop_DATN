using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.ToTable("InventoryTransaction");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Note).HasMaxLength(500).HasColumnType("nvarchar(500)");
            builder.HasOne(x => x.InventoryBatch).WithMany(x => x.InventoryTransactions).HasForeignKey(x => x.InventoryBatchId);
        }
    }
}
