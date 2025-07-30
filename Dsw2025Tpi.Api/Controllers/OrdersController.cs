using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService? _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] OrderRequest request)
    {
        var createdOrder = await _orderService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdOrder.Id },
            createdOrder
        );
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        return Ok(); // Placeholder, se implementará más adelante
    }

}
