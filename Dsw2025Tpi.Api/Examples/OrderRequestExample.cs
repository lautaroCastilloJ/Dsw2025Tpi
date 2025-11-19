using Dsw2025Tpi.Application.Dtos.Orders;
using Swashbuckle.AspNetCore.Filters;

namespace Dsw2025Tpi.Api.Examples;

public class OrderRequestExample : IExamplesProvider<OrderRequest>
{
    public OrderRequest GetExamples()
    {
        return new OrderRequest(
            ShippingAddress: "Calle Principal 123, Apartamento 4B, Ciudad, Provincia 12345",
            BillingAddress: "Calle Secundaria 456, Apartamento 2A, Ciudad, Provincia 12345",
            Notes: "Entregar después de las 18:00",
            OrderItems: new List<OrderItemRequest>
            {
                new OrderItemRequest(
                    ProductId: Guid.Parse(""),
                    Quantity: 2
                )
            }
        );
    }
}
