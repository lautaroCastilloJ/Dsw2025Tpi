using Dsw2025Tpi.Api.Examples;
using Dsw2025Tpi.Application.Dtos.Orders;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Exceptions.OrderExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }


    // ----------------------------------------------------------------------
    // 6. Crear una nueva orden: POST /api/orders 
    // Solo rol Cliente puede crear órdenes
    // ----------------------------------------------------------------------
    [HttpPost]
    [Authorize(Roles = AppRoles.Cliente)]
    [SwaggerRequestExample(typeof(OrderRequest), typeof(OrderRequestExample))]
    public async Task<IActionResult> Create([FromBody] OrderRequest request)
    {
        // 1) Obtener CustomerId del token
        var customerIdClaim = User.FindFirst("customerId")?.Value;

        if (string.IsNullOrWhiteSpace(customerIdClaim))
            return Forbid(); // token inválido o sin customerId

        var customerId = Guid.Parse(customerIdClaim);

        // 2) Crear la orden con un customerId confiable
        var created = await _orderService.CreateOrderAsync(customerId, request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    // ----------------------------------------------------------------------
    // 7. Obtener orden por ID: GET /api/orders/{id}
    // ----------------------------------------------------------------------
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{AppRoles.Cliente},{AppRoles.Administrador}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        return Ok(result);
    }

    // ----------------------------------------------------------------------
    // 8. Obtener mis órdenes (Cliente): GET /api/orders/my-orders
    // Solo puede ver sus propias órdenes, customerId viene del token
    // ----------------------------------------------------------------------
    [HttpGet("my-orders")]
    [Authorize(Roles = AppRoles.Cliente)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetMyOrders(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Obtener customerId del token
        var customerIdClaim = User.FindFirst("customerId")?.Value;
        if (!Guid.TryParse(customerIdClaim, out var customerId) || customerId == Guid.Empty)
            return Unauthorized(new { error = "No se pudo resolver el cliente desde el token." });

        var orders = await _orderService.GetAllOrdersAsync(status, customerId, pageNumber, pageSize);

        return Ok(orders);
    }

    // ----------------------------------------------------------------------
    // 9. Obtener todas las órdenes (Administrador): GET /api/orders/admin
    // Puede filtrar por customerId, status, con paginación
    // ----------------------------------------------------------------------
    [HttpGet("admin")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAll(
        [FromQuery] string? status,
        [FromQuery] Guid? customerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var orders = await _orderService.GetAllOrdersAsync(status, customerId, pageNumber, pageSize);

        return Ok(orders);
    }

    // ----------------------------------------------------------------------
    // 10. Actualizar estado de orden (Administrador): PUT /api/orders/{id}/status
    // Solo administradores pueden cambiar el estado de una orden
    // ----------------------------------------------------------------------
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = AppRoles.Administrador)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var updated = await _orderService.UpdateOrderStatusAsync(id, request.NewStatus);

        return Ok(updated);
    }
}