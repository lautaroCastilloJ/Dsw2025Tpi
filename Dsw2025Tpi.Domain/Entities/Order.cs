using Dsw2025Tpi.Domain.Common;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using System.Net;

namespace Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
    public DateTime Date { get; private set; } = DateTime.UtcNow;
    public string ShippingAddress { get; private set; } = default!;
    public string BillingAddress { get; private set; } = default!;
    public string? Notes { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(i => i.Subtotal);

    private Order() { }

    public static Order Create(
        Guid customerId,
        string shippingAddress,
        string billingAddress,
        string? notes,
        IEnumerable<OrderItem> items,
        IDictionary<Guid, Product> stockSource)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("Dirección de envío requerida.");
        if (string.IsNullOrWhiteSpace(billingAddress))
            throw new ArgumentException("Dirección de facturación requerida.");
        if (!items.Any())
            throw new ArgumentException("La orden debe tener al menos un item.");

        foreach (var item in items)
        {
            if (!stockSource.TryGetValue(item.ProductId, out var product))
                throw new ArgumentException($"Producto inexistente: {item.ProductId}");

            if (!product.HasStock(item.Quantity))
                throw new InvalidOperationException($"Stock insuficiente para el producto: {item.ProductName}");

            product.DecreaseStock(item.Quantity); // Solo si pasó las validaciones
        }

        var order = new Order
        {
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress,
            Notes = notes
        };

        order._items.AddRange(items);
        return order;
    }


    public void ChangeStatus(OrderStatus newStatus)
    {
        if (newStatus == Status)
            return;

        if (Status == OrderStatus.Cancelled || Status == OrderStatus.Delivered)
            throw new InvalidOperationException("No se puede cambiar el estado desde el actual.");

        Status = newStatus;
    }

}