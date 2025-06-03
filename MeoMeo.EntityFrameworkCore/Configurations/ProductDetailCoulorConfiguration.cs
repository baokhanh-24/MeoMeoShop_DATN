using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductDetailColourConfiguration : IEntityTypeConfiguration<ProductDetailColour>
    {
        public void Configure(EntityTypeBuilder<ProductDetailColour> builder)
        {
            builder.ToTable("ProductDetailColour");
            builder.HasKey(x => x.ProductDetailId);
            builder.HasKey(x => x.ColourId);
            builder.HasOne(x => x.ProductDetail).WithMany(x => x.ProductDetailColours).HasForeignKey(x => x.ProductDetailId);
            builder.HasOne(x => x.Colour).WithMany(x => x.ProductDetailColours).HasForeignKey(x => x.ColourId);
        }
    }
}
