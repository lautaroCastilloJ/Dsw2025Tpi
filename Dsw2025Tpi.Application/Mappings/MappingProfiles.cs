using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Orders;
using Dsw2025Tpi.Application.Dtos.Products;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Product
        CreateMap<Product, ProductResponse>();

        CreateMap<ProductRequest, Product>()
            .ConstructUsing(src => Product.Create(
                src.Sku,
                src.InternalCode,
                src.Name,
                src.Description ?? string.Empty,
                src.CurrentUnitPrice,
                src.StockQuantity
            ));

        // Order (solo lectura)
        CreateMap<Order, OrderResponse>();
        CreateMap<OrderItem, OrderItemResponse>();
    }
}
