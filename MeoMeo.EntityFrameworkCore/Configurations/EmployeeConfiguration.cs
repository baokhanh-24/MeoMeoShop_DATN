using MeoMeo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeoMeo.EntityFrameworkCore.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employee");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).HasColumnType("nvarchar(100)");
            builder.Property(x => x.Code).HasMaxLength(10).HasColumnType("varchar(10)");
            builder.Property(x => x.PhoneNumber).HasMaxLength(12).HasColumnType("varchar(12)");
            builder.Property(x => x.Address).HasMaxLength(255).HasColumnType("nvarchar(255)");
            builder.HasOne(x => x.User).WithOne(x => x.Employee).HasForeignKey<Employee>(x => x.UserId);
        }
    }
}
