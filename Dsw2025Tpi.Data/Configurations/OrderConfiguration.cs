using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Tabla
        builder.ToTable("Orders");

        // Clave primaria
        builder.HasKey(o => o.Id);

        // Columnas o propiedades de la tabla
        builder.Property(o => o.Date)
            .IsRequired();

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(o => o.BillingAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(o => o.Notes)
            .HasMaxLength(500);

        builder.Property(o => o.Status)
            .IsRequired();
    }
}
