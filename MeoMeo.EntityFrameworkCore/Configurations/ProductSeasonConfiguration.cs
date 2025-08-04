using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ProductSeasonConfiguration : IEntityTypeConfiguration<ProductSeason>
    {
        public void Configure(EntityTypeBuilder<ProductSeason> builder)
        {
            builder.ToTable("ProductSeasons");
            builder.HasKey(x => new { x.SeasonId, x.ProductId });
            builder.HasOne(x => x.Season).WithMany(x => x.ProductSeasons).HasForeignKey(x => x.SeasonId);
            builder.HasOne(x => x.Product).WithMany(x => x.ProductSeasons).HasForeignKey(x => x.ProductId);
        }
    }
}
