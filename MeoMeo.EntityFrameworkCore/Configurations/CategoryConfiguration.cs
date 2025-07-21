using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasMany(c => c.ProductCategories)
            .WithOne(pc => pc.Category)
            .HasForeignKey(pc => pc.CategoryId);
    }
}