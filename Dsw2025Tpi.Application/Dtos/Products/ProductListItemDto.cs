using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Products;

public sealed record ProductListItemDto(
    Guid Id,
    string Sku,
    string Name,
    decimal CurrentUnitPrice,
    bool IsActive);