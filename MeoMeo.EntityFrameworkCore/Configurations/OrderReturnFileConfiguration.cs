using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class OrderReturnFileConfiguration : IEntityTypeConfiguration<OrderReturnFile>
    {
        public void Configure(EntityTypeBuilder<OrderReturnFile> builder)
        {
            builder.ToTable("OrderReturnFiles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Url).IsRequired();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.ContentType).IsRequired();
            builder.HasOne(x => x.OrderReturn).WithMany(x => x.Files).HasForeignKey(x => x.OrderReturnId);
        }
    }
}


