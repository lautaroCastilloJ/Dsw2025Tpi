using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class OrderInsufficientStockException : ExceptionBase
{
    public Guid ProductId { get; }
    public string ProductName { get; }
    public int RequestedQuantity { get; }
    public int AvailableQuantity { get; }

    public OrderInsufficientStockException(
        Guid productId,
        string productName,
        int requestedQuantity,
        int availableQuantity)
        : base("ORDER_INSUFFICIENT_STOCK",
            $"Stock insuficiente para el producto '{productName}'. Se solicitan {requestedQuantity} unidades, pero solo hay {availableQuantity} disponibles.")
    {
        ProductId = productId;
        ProductName = productName;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }
}
