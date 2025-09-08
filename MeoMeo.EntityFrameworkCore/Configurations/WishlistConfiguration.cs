using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlist");
            builder.Property(x => x.CustomerId).IsRequired();
            builder.Property(x => x.ProductDetailId).IsRequired();
            builder.HasKey(x => new { x.CustomerId, x.ProductDetailId });
            builder.HasOne(x => x.Customers)
                .WithMany(x=>x.Wishlists)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.ProductDetails)
                .WithMany(x=>x.Wishlists)
                .HasForeignKey(x => x.ProductDetailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


