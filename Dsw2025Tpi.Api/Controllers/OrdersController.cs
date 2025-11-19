using Dsw2025Tpi.Api.Examples;
using Dsw2025Tpi.Application.Dtos.Orders;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Exceptions.OrderExceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

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

    [HttpPost]
    [Authorize(Roles = "Cliente")] // SOLO clientes pueden crear órdenes
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

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        return Ok(result);
    }
}