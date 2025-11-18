using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Products;

public sealed class ProductListItemDto
{
    public Guid Id { get; init; }
    public string Sku { get; init; } = default!;
    public string Name { get; init; } = default!;
    public decimal CurrentUnitPrice { get; init; }
    public bool IsActive { get; init; }
}