using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class ResetPasswordHistoryConfiguration : IEntityTypeConfiguration<ResetPasswordHistory>
    {
        public void Configure(EntityTypeBuilder<ResetPasswordHistory> builder)
        {
            builder.ToTable("ResetPasswordHistories");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(5).HasColumnType("varchar(5)");
            builder.HasOne(x => x.User).WithMany(x => x.ResetPasswordHistories).HasForeignKey(x => x.UserId);
        }
    }
}
