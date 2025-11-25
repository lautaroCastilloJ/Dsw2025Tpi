using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Orders;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Pagination;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.Common;
using Dsw2025Tpi.Domain.Exceptions.CustomerExceptions;
using Dsw2025Tpi.Domain.Exceptions.OrderExceptions;
using Dsw2025Tpi.Domain.Exceptions.ProductExceptions;
using Dsw2025Tpi.Domain.Interfaces;

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

    // ===========================================================
    // 1) CREAR ORDEN
    // ===========================================================
    public async Task<OrderResponse> CreateOrderAsync(Guid customerId, OrderRequest request)
    {
        if (request.OrderItems is null || !request.OrderItems.Any())
            throw new OrderWithoutItemsException();

        // Validar cliente
        var customer = await _customerRepository.GetById(customerId);
        if (customer is null)
            throw new InvalidOrderCustomerException();

        if (!customer.IsActive)
            throw new CustomerInactiveException(customerId);

        // Obtener productos involucrados
        var productIds = request.OrderItems
            .Select(i => i.ProductId)
            .Distinct()
            .ToList();

        var products = await _productRepository
            .GetFiltered(p => productIds.Contains(p.Id))
            ?? new List<Product>();

        var productDict = products.ToDictionary(p => p.Id);

        // Validar existencia
        foreach (var pid in productIds)
        {
            if (!productDict.ContainsKey(pid))
                throw new ProductNotFoundException(pid);
        }

        // Validar stock
        foreach (var item in request.OrderItems)
        {
            var prod = productDict[item.ProductId];

            if (!prod.IsActive)
                throw new ProductInactiveException();

            if (!prod.HasSufficientStock(item.Quantity))
            {
                throw new OrderInsufficientStockException(
                    prod.Id,
                    prod.Name,
                    item.Quantity,
                    prod.StockQuantity
                );
            }
        }

        // Crear orden
        var order = Order.Create(
            customerId,
            request.ShippingAddress,
            request.BillingAddress,
            request.Notes
        );

        // Agregar items con snapshot de nombre y precio
        foreach (var itemReq in request.OrderItems)
        {
            var prod = productDict[itemReq.ProductId];

            order.AddItem(
                prod.Id,
                prod.Name,
                itemReq.Quantity,
                prod.CurrentUnitPrice
            );

            prod.DecreaseStock(itemReq.Quantity);
            await _productRepository.Update(prod);
        }

        order.ValidateHasItems();
        await _orderRepository.Add(order);

        return _mapper.Map<OrderResponse>(order);
    }

    // ===========================================================
    // 2) OBTENER ORDEN POR ID
    // ===========================================================
    public async Task<OrderResponse> GetOrderByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetById(id, "Items,Customer"); // Include Items y Customer

        if (order is null)
            throw new OrderNotFoundException(id);

        return _mapper.Map<OrderResponse>(order);
    }

    // ===========================================================
    // 3) LISTAR TODAS LAS ÓRDENES
    // ===========================================================
    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(
        string? status,
        Guid? customerId,
        int pageNumber,
        int pageSize)
    {
        var query = _orderRepository.GetAllQueryable("Items,Customer"); // Include Items y Customer

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                throw new InvalidOrderStatusException(status);

            query = query.Where(o => o.Status == parsedStatus);
        }

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        // Normalizar paginación
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
        const int maxSize = 100;
        if (pageSize > maxSize) pageSize = maxSize;

        // Aplicar paginación
        query = query
            .OrderByDescending(o => o.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var orders = query.ToList();

        return orders.Select(o => _mapper.Map<OrderResponse>(o));
    }

    // ===========================================================
    // 4) ACTUALIZAR ESTADO DE ORDEN
    // ===========================================================
    public async Task<OrderResponse> UpdateOrderStatusAsync(Guid id, string newStatus)
    {
        var order = await _orderRepository.GetById(id, "Items,Customer"); // Include Items y Customer
        if (order is null)
            throw new OrderNotFoundException(id);

        if (!Enum.TryParse<OrderStatus>(newStatus, true, out var status))
            throw new InvalidOrderStatusException(newStatus);

        switch (status)
        {
            case OrderStatus.Processing: order.MarkAsProcessing(); break;
            case OrderStatus.Shipped: order.MarkAsShipped(); break;
            case OrderStatus.Delivered: order.MarkAsDelivered(); break;
            case OrderStatus.Cancelled: order.Cancel(); break;
            default:
                throw new InvalidOrderStatusTransitionException(order.Status, status);
        }

        await _orderRepository.Update(order);

        return _mapper.Map<OrderResponse>(order);
    }

    // ===========================================================
    // 5) PAGINAR ÓRDENES (para Admin o Cliente)
    // ===========================================================
    public async Task<PagedResult<OrderListItemDto>> GetPagedAsync(
        FilterOrder filter,
        CancellationToken cancellationToken = default)
    {
        var query = _orderRepository.GetAllQueryable("Items,Customer"); // Include Items y Customer

        // Filtrar por estado
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (!Enum.TryParse<OrderStatus>(filter.Status, true, out var parsedStatus))
                throw new InvalidOrderStatusException(filter.Status);

            query = query.Where(o => o.Status == parsedStatus);
        }

        // Filtrar por Customer
        if (filter.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == filter.CustomerId.Value);
        }

        // Filtro de búsqueda (opcional)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.ToLower();
            query = query.Where(o =>
                   o.ShippingAddress.ToLower().Contains(searchTerm)
                || o.BillingAddress.ToLower().Contains(searchTerm)
                || (o.Notes != null && o.Notes.ToLower().Contains(searchTerm)));
        }

        // Contar total
        var total = query.Count();

        // Normalizar paginación
        var pageNumber = filter.PageNumber.HasValue && filter.PageNumber.Value > 0 ? filter.PageNumber.Value : 1;
        var pageSize = filter.PageSize.HasValue && filter.PageSize.Value > 0 ? filter.PageSize.Value : 10;
        const int maxSize = 100;
        if (pageSize > maxSize) pageSize = maxSize;

        // Aplicar paginación
        var items = query
            .OrderByDescending(o => o.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dto = items
            .Select(o => _mapper.Map<OrderListItemDto>(o))
            .ToList();

        return PagedResult<OrderListItemDto>.Create(
            dto,
            total,
            pageNumber,
            pageSize
        );
    }
}
