using Dsw2025Tpi.Domain.Enums;

namespace Dsw2025Tpi.Application.Dtos.Payments;

public sealed record PaymentDetailResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string MercadoPagoId { get; init; } = string.Empty;
    public string PreferenceId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public PaymentStatus Status { get; init; }
    public PaymentMethod? Method { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? PayerEmail { get; init; }
    public string? StatusDetail { get; init; }
}
