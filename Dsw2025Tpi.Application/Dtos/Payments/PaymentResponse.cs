using Dsw2025Tpi.Domain.Enums;

namespace Dsw2025Tpi.Application.Dtos.Payments;

public sealed record PaymentResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string PreferenceId { get; init; } = string.Empty;
    public string InitPoint { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public PaymentStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}
