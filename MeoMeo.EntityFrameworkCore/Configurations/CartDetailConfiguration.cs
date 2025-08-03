using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class CartDetailConfiguration : IEntityTypeConfiguration<CartDetail>
    {
        public void Configure(EntityTypeBuilder<CartDetail> builder)
        {
            builder.ToTable("CartDetails");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Cart).WithMany(x => x.CartDetails).HasForeignKey(x => x.CartId);
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.CartDetails).HasForeignKey(x => x.ProductDetailId);
            builder.HasOne(x => x.PromotionDetails).WithMany(x => x.CartDetails).HasForeignKey(x => x.PromotionDetailId);
        }
    }
}
