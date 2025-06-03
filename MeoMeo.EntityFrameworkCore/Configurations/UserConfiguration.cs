using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserName).HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(x => x.PasswordHash).HasMaxLength(100).HasColumnType("varchar(100)");
            builder.Property(x => x.Avatar).HasMaxLength(255).HasColumnType("varchar(255)");
            builder.Property(x => x.Email).HasMaxLength(255).HasColumnType("varchar(255)");
        }
    }
}
