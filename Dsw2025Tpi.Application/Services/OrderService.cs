using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Dsw2025Tpi.Application.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public OrderService(
        IRepository<Order> orderRepository,
        IRepository<Product> productRepository,
        IRepository<Customer> customerRepository,
        IMapper mapper,
        UserManager<AppUser> userManager)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
        _userManager = userManager;
    }

 /*   private async Task ValidarUsuarioClienteAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            throw new ArgumentException("El ID proporcionado no pertenece a un usuario registrado.");

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(AppRoles.Cliente))
            throw new UnauthorizedAccessException("Solo los usuarios con rol 'Cliente' pueden hacer pedidos.");
    }*/


    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        // 1. Validar que el cliente exista
        var customer = await _customerRepository.GetById(request.CustomerId);
        if (customer is null)
            throw new ArgumentException($"No existe el cliente con ID: {request.CustomerId}");


        // 3. Validar que haya ítems
        if (request.OrderItems == null || !request.OrderItems.Any())
            throw new ArgumentException("La orden debe contener al menos un ítem.");

        // 4. Obtener y validar productos
        var productIds = request.OrderItems.Select(i => i.ProductId).Distinct();
        var products = await _productRepository.GetFiltered(p => productIds.Contains(p.Id));

        if (products == null || products.Count() != productIds.Count())
            throw new ArgumentException("Uno o más productos no existen.");

        var productMap = products.ToDictionary(p => p.Id);

        var orderItems = request.OrderItems.Select(item =>
        {
            var product = productMap[item.ProductId];

            if (!product.IsActive)
                throw new InvalidOperationException($"El producto '{product.Name}' está deshabilitado.");

            if (!product.HasStock(item.Quantity))
                throw new InvalidOperationException($"Stock insuficiente para el producto: {product.Name}");

            product.DecreaseStock(item.Quantity);
            return OrderItem.Create(product.Id, product.Name, product.CurrentUnitPrice, item.Quantity);
        }).ToList();

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



    public async Task<IEnumerable<OrderResponse>> GetAllAsync(string? status, Guid? customerId, int pageNumber, int pageSize)
    {
        var query = _orderRepository.GetAllQueryable("Customer", "Items");

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                throw new ArgumentException($"El estado '{status}' no es válido. Los valores posibles son: Pending, Paid, Cancelled, Shipped.");

            query = query.Where(o => o.Status == parsedStatus);
        }

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        var paginatedOrders = await query
            .OrderByDescending(o => o.Date)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return paginatedOrders.Select(order => new OrderResponse(
            Id: order.Id,
            CustomerId: order.CustomerId,
            Date: order.Date,
            ShippingAddress: order.ShippingAddress,
            BillingAddress: order.BillingAddress,
            Notes: order.Notes,
            Status: order.Status.ToString(),
            TotalAmount: order.TotalAmount,
            Items: order.Items.Select(i => new OrderItemResponse(
                ProductId: i.ProductId,
                ProductName: i.ProductName,
                UnitPrice: i.UnitPrice,
                Quantity: i.Quantity,
                Subtotal: i.UnitPrice * i.Quantity
            )).ToList()
        ));
    }

    public async Task<OrderResponse> UpdateStatusAsync(Guid id, string newStatus)
    {
        var order = await _orderRepository.GetById(id);

        if (order is null)
            throw new EntityNotFoundException("Orden", id);

        if (!Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var parsedStatus))
            throw new ArgumentException($"El estado '{newStatus}' no es válido. Usa: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");


        order.ChangeStatus(parsedStatus);
        await _orderRepository.Update(order);

        return _mapper.Map<OrderResponse>(order);
    }


    public async Task<OrderResponse> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository
    .GetAllQueryable(nameof(Order.Items))
    .FirstOrDefaultAsync(o => o.Id == id);


        if (order == null)
            throw new EntityNotFoundException("Orden", id);

        return _mapper.Map<OrderResponse>(order);

    }



}



/*
     public async Task<OrderResponse> GetByIdAsync(Guid id, ClaimsPrincipal user)
    {
        var order = await _orderRepository
            .GetAllQueryable(nameof(Order.Items))
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            throw new EntityNotFoundException("Orden", id);

        // Si es cliente, debe ser el dueño de la orden
        if (user.IsInRole(AppRoles.Cliente))
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out var parsedId) || order.CustomerId != parsedId)
                throw new UnauthorizedAccessException("No tiene permiso para acceder a esta orden.");
        }

        // Si es administrador, no se aplica restricción adicional
        return _mapper.Map<OrderResponse>(order);
    }
 
 */