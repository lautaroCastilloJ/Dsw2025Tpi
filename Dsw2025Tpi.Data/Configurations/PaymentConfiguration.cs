using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dsw2025Tpi.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderId)
            .IsRequired();

        builder.Property(p => p.MercadoPagoId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.PreferenceId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Method)
            .HasConversion<int?>()
            .IsRequired(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.ProcessedAt)
            .IsRequired(false);

        builder.Property(p => p.ExternalReference)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(p => p.PayerEmail)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(p => p.StatusDetail)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.MercadoPagoId);
        builder.HasIndex(p => p.PreferenceId);
    }
}
