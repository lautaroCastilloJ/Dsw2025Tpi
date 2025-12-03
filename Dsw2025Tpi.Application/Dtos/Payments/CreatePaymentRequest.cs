namespace Dsw2025Tpi.Application.Dtos.Payments;

public sealed record CreatePaymentRequest
{
    public Guid OrderId { get; init; }
    public string? BackUrl { get; init; }
}
