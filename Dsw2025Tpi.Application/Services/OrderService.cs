using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;


namespace Dsw2025Tpi.Application.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IRepository<Order> orderRepository,
        IRepository<Product> productRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        if (request.OrderItems == null || !request.OrderItems.Any())
            throw new ArgumentException("La orden debe contener al menos un ítem.");

        var productIds = request.OrderItems.Select(i => i.ProductId).Distinct();
        var products = await _productRepository.GetFiltered(p => productIds.Contains(p.Id));

        if (products == null || products.Count() != productIds.Count())
            throw new ArgumentException("Uno o más productos no existen.");

        var productMap = products.ToDictionary(p => p.Id);

        var orderItems = request.OrderItems.Select(item =>
        {
            var product = productMap[item.ProductId];
            if (!product.HasStock(item.Quantity))
                throw new InvalidOperationException($"Stock insuficiente para el producto: {product.Name}");

            product.DecreaseStock(item.Quantity);

            return OrderItem.Create(product.Id, product.Name, product.CurrentUnitPrice, item.Quantity);
        });

        var order = Order.Create(
            request.CustomerId,
            request.ShippingAddress,
            request.BillingAddress,
            request.Notes,
            orderItems,
            productMap
        );

        await _orderRepository.Add(order);

        var created = await _orderRepository.GetById(order.Id, "Items");
        return _mapper.Map<OrderResponse>(created);
    }
}
