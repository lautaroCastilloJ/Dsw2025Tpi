﻿using System;
using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Entities;

public class Product : EntityBase
{
    
    public string Sku { get; set; }
    public string InternalCode { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal CurrentUnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }

    // Relacion: 1 producto puede aparecer en varios items
    public ICollection<OrderItem> OrderItems { get; set; }
}
