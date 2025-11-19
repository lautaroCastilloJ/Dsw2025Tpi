using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Orders;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Pagination;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.OrderExceptions;
using Dsw2025Tpi.Domain.Exceptions.ProductExceptions;
using Dsw2025Tpi.Domain.Exceptions.CustomerExceptions;
using Dsw2025Tpi.Domain.Interfaces;
using System;

namespace Dsw2025Tpi.Application.Services;

public sealed class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IRepository<Order> orderRepository,
        IRepository<Product> productRepository,
        IRepository<Customer> customerRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<OrderResponse> CreateOrderAsync(Guid customerId, OrderRequest request)
    {
        if (request.OrderItems is null || !request.OrderItems.Any())
            throw new OrderWithoutItemsException();

        // 0) Validar Customer real
        var customer = await _customerRepository.GetById(customerId);
        if (customer is null)
            throw new InvalidOrderCustomerException();

        if (!customer.IsActive)
            throw new CustomerInactiveException(customerId);

        // 1) Traer productos necesarios
        var productIds = request.OrderItems
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        var products = await _productRepository.GetFiltered(p => productIds.Contains(p.Id))
                       ?? Enumerable.Empty<Product>();

        var productDict = products.ToDictionary(p => p.Id);

        foreach (var id in productIds)
        {
            if (!productDict.ContainsKey(id))
                throw new ProductNotFoundException(id);
        }

        // 2) Validar stock + producto activo
        foreach (var item in request.OrderItems)
        {
            var product = productDict[item.ProductId];

            if (!product.IsActive)
                throw new ProductInactiveException();

            if (!product.HasSufficientStock(item.Quantity))
            {
                throw new OrderInsufficientStockException(
                    product.Id,
                    product.Name,
                    item.Quantity,
                    product.StockQuantity
                );
            }
        }

        // 3) Crear orden
        var order = Order.Create(
            customerId,
            request.ShippingAddress,
            request.BillingAddress,
            request.Notes
        );

        // 4) Agregar ítems con snapshot de precio y nombre
        foreach (var it in request.OrderItems)
        {
            var prod = productDict[it.ProductId];

            order.AddItem(
                prod.Id,
                prod.Name,              // snapshot del nombre
                it.Quantity,
                prod.CurrentUnitPrice   // snapshot del precio
            );

            prod.DecreaseStock(it.Quantity);
            await _productRepository.Update(prod);
        }

        order.ValidateHasItems();

        // 5) Persistir
        await _orderRepository.Add(order);

        return _mapper.Map<OrderResponse>(order);
    }



}
