using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Entities;

public class OrderItem : EntityBase
{

    public int Quantity { get; private set; }
    public string ProductName { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public Guid OrderId { get; private set; } // Foreign Key explícita
    public Order Order { get; private set; } = default!; // Navegación inversa
    public Guid ProductId { get; private set; } // clave foranea

    private OrderItem() { }

    public static OrderItem Create(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (unitPrice <= 0) throw new ArgumentException("Precio inválido.");
        if (quantity <= 0) throw new ArgumentException("Cantidad inválida.");

        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }
}
