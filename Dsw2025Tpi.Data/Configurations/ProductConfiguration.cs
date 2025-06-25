using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Nombre de la Tabla
        builder.ToTable("Products");

        // GUID como PK
        builder.HasKey(p => p.Id);

        // SKU obligatorio y único
        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(p => p.Sku).IsUnique();

        // Name obligatorio
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.InternalCode).IsRequired().HasMaxLength(100);

        // Price mayor a 0 (regla de validación, pero aquí lo dejamos preparado) en capa de aplicacion
        builder.Property(p => p.CurrentUnitPrice)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        // Stock no negativo (también se valida por código) en capa de aplicaion
        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        // Opcionales
        
        builder.Property(p => p.Description).HasMaxLength(500);
    }
}
