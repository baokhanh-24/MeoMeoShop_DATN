using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Function)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Command)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.PermissionGroupId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.PermissionGroup)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.PermissionGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.RolePermissions)
            .WithOne(x => x.Permission)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}