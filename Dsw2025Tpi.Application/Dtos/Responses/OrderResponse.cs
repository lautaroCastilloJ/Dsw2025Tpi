using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Responses;
public record OrderResponse(
    Guid Id,
    DateTime Date,
    string Status,
    Guid CustomerId,
    string CustomerName,
    string ShippingAddress,
    string BillingAddress,
    string? Notes,
    decimal TotalAmount,
    List<OrderItemResponse> OrderItems
);
