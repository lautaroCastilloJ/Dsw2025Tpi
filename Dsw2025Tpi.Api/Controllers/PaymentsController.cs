using Dsw2025Tpi.Application.Dtos.Payments;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Api.Extensions;
using Dsw2025Tpi.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Cliente)]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating payment for Order {OrderId}", request.OrderId);

        var customerId = HttpContext.GetCustomerId();
        _logger.LogDebug("CustomerId from token: {CustomerId}", customerId);

        var response = await _paymentService.CreatePaymentPreferenceAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetPaymentById),
            new { paymentId = response.Id },
            response);
    }

    [HttpGet("order/{orderId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(PaymentDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentByOrder(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting payment for Order {OrderId}", orderId);

        var response = await _paymentService.GetPaymentByOrderIdAsync(orderId, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{paymentId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(PaymentDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting payment {PaymentId}", paymentId);

        var response = await _paymentService.GetPaymentByIdAsync(paymentId, cancellationToken);

        return Ok(response);
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> WebhookNotification(
        [FromQuery(Name = "id")] string? id,
        [FromQuery(Name = "topic")] string? topic,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received webhook notification - ID: {Id}, Topic: {Topic}", id, topic);

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(topic))
        {
            _logger.LogWarning("Invalid webhook notification received");
            return BadRequest("Invalid notification");
        }

        try
        {
            await _paymentService.ProcessWebhookNotificationAsync(id, topic, cancellationToken);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook notification");
            return StatusCode(500, "Error processing notification");
        }
    }
}
