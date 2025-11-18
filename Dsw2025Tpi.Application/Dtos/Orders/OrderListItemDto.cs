namespace Dsw2025Tpi.Application.Dtos.Orders;

public sealed class OrderListItemDto
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public string Status { get; init; } = default!;
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
}
