using System;
using Dsw2025Tpi.Domain.Common;
using Dsw2025Tpi.Domain.Enums;

namespace Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }

    // Relación con Customer: Del lado de Order hacia Cliente es 1 a 1
    public Guid CustomerId { get; set; } // clave foranea que referencia que cliente hizo tal orden
    public Customer Customer { get; set; } // propiedad de navegación, que permite acceder directamente al objeto Customer relacionado desde una instancia de Order.

    // Estado de la orden
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Navegación a ítems
    public ICollection<OrderItem> OrderItems { get; set; }
}
