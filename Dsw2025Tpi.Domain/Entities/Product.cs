using System;
using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Entities;

public class Product : EntityBase
{
    public required string Sku { get; set; }
    public required string InternalCode { get; set; }  
    public required string Name { get; set; }
    public required string Description { get; set; }    
    public decimal CurrentUnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
