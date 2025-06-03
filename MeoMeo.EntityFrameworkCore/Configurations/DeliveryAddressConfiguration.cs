using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class DeliveryAddressConfiguration : IEntityTypeConfiguration<DeliveryAddress>
    {
        public void Configure(EntityTypeBuilder<DeliveryAddress> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.PhoneNumber).HasMaxLength(12).HasColumnType("varchar(12)");
            builder.Property(x => x.Address).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.HasOne(x => x.Customers).WithMany(x => x.DeliveryAddresses).HasForeignKey(x => x.CustomerId);
            builder.HasOne(x => x.Province).WithMany(x => x.DeliveryAddresses).HasForeignKey(x => x.ProvinceId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.District).WithMany(x => x.DeliveryAddresses).HasForeignKey(x => x.DistrictId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Commune).WithMany(x => x.DeliveryAddresses).HasForeignKey(x => x.CommuneId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
