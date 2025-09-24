using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class PromotionDetailConfiguration : IEntityTypeConfiguration<PromotionDetail>
    {
        public void Configure(EntityTypeBuilder<PromotionDetail> builder)
        {
            builder.ToTable("PromotionDetails");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Note).HasMaxLength(1000).HasColumnType("nvarchar(1000)").IsRequired(false);
            builder.HasOne(x => x.Promotion).WithMany(x => x.PromotionDetails).HasForeignKey(x => x.PromotionId);
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.PromotionDetails).HasForeignKey(x => x.ProductDetailId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
