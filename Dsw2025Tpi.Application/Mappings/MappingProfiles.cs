using AutoMapper;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;

namespace Dsw2025Tpi.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Product
        CreateMap<Product, ProductResponse>();
        CreateMap<ProductRequest, Product>();

        // Order
        CreateMap<Order, OrderResponse>();
        CreateMap<OrderItem, OrderItemResponse>();
    }
}
