using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Entities;

public class OrderItem : EntityBase
{
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }

    // Relaciones 
    public Guid OrderId { get; set; } // clave foranea
    public Order Order { get; set; } // propiedad de navegación que permite acceder directamente al objeto Order relacionado desde una instancia de OrderItem.

    public Guid ProductId { get; set; } // clave foranea
    public Product Product { get; set; }
}
