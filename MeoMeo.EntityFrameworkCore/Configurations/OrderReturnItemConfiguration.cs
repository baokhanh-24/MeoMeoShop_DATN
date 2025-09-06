using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class OrderReturnItemConfiguration : IEntityTypeConfiguration<OrderReturnItem>
    {
        public void Configure(EntityTypeBuilder<OrderReturnItem> builder)
        {
            builder.ToTable("OrderReturnItems");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.OrderReturn).WithMany(x => x.Items).HasForeignKey(x => x.OrderReturnId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.OrderDetail).WithMany().HasForeignKey(x => x.OrderDetailId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}


