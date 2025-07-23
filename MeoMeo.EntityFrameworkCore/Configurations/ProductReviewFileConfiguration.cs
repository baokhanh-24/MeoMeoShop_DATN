using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductReviewFileConfiguration : IEntityTypeConfiguration<ProductReviewFile>
    {
        public void Configure(EntityTypeBuilder<ProductReviewFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FileName).IsRequired();
            builder.Property(x => x.FileUrl).IsRequired();
            builder.Property(x => x.FileType).IsRequired();
            builder.HasOne(x => x.ProductReview)
                .WithMany(x => x.ProductReviewFiles)
                .HasForeignKey(x => x.ProductReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 