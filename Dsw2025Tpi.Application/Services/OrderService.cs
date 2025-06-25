using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Interfaces.Repositories;

namespace Dsw2025Tpi.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderResponse?> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.Orders.GetById(id, "OrderItems", "Customer");
            return order is null ? null : _mapper.Map<OrderResponse>(order);
        }

        public async Task<IEnumerable<OrderResponse>> GetAllAsync(
            string? status = null,
            Guid? customerId = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var orders = await _unitOfWork.Orders.GetAll("OrderItems", "Customer") ?? new List<Order>();

            if (!string.IsNullOrEmpty(status))
                orders = orders.Where(o => o.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));

            if (customerId.HasValue)
                orders = orders.Where(o => o.CustomerId == customerId.Value);

            // Paginación
            orders = orders.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return orders.Select(o => _mapper.Map<OrderResponse>(o));
        }

        public async Task<OrderResponse> CreateAsync(OrderRequest request)
        {
            // Validar stock y calcular precios
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var itemReq in request.OrderItems)
            {
                var product = await _unitOfWork.Products.GetById(itemReq.ProductId);
                if (product is null)
                    throw new EntityNotFoundException("Producto", itemReq.ProductId);

                if (itemReq.Quantity > product.StockQuantity)
                    throw new InsufficientStockException(product.Name, product.Sku);

                // Restar stock y fijar precio en el momento de la orden
                product.StockQuantity -= itemReq.Quantity;

                var unitPrice = product.CurrentUnitPrice;
                var subtotal = unitPrice * itemReq.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemReq.Quantity,
                    UnitPrice = unitPrice,
                    Subtotal = subtotal
                });

                totalAmount += subtotal;

                // Actualizar el producto en la base (stock)
                await _unitOfWork.Products.Update(product);
            }

            var order = new Order
            {
                CustomerId = request.CustomerId,
                ShippingAddress = request.ShippingAddress,
                BillingAddress = request.BillingAddress,
                Notes = request.Notes,
                Date = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = orderItems,
                TotalAmount = totalAmount
            };

            await _unitOfWork.Orders.Add(order);
            await _unitOfWork.SaveChangesAsync();

            // Mapear y retornar OrderResponse
            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse?> UpdateStatusAsync(Guid orderId, string newStatus)
        {
            var order = await _unitOfWork.Orders.GetById(orderId, "OrderItems", "Customer");
            if (order is null)
                return null;

            if (Enum.TryParse(typeof(OrderStatus), newStatus, true, out var statusObj))
            {
                order.Status = (OrderStatus)statusObj;
                await _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<OrderResponse>(order);
            }

            return null;
        }
    }
}
