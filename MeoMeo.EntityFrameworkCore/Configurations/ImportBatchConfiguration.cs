using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
    {
        public void Configure(EntityTypeBuilder<ImportBatch> builder)
        {
            builder.ToTable("ImportBatches");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(20).HasColumnType("varchar(20)");
            builder.Property(x => x.Note).HasMaxLength(500).HasColumnType("nvarchar(500)");
            builder.Property(x => x.ImportDate).HasColumnType("datetime2");

            // Index for better performance
            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
