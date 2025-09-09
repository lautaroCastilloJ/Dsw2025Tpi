using Dsw2025Tpi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2025Tpi.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.Property(c => c.Id)
               .ValueGeneratedNever(); // ← Esto le dice a EF que respete el valor que le estás dando

        builder.Property(c => c.Email)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
               .HasMaxLength(20);
    }
}
