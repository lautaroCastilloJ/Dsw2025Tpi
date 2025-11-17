using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Order;

public record OrderRequest(
    Guid CustomerId,
    string ShippingAddress,
    string BillingAddress,
    string? Notes,
    List<OrderItemRequest> OrderItems
);
