using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Payments;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.OrderExceptions;
using Dsw2025Tpi.Domain.Exceptions.PaymentExceptions;
using Dsw2025Tpi.Domain.Exceptions.ProductExceptions;
using Dsw2025Tpi.Domain.Interfaces;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IRepository<Order> _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentService> _logger;
    private readonly string _accessToken;
    private readonly string _notificationUrl;

    public PaymentService(
        IRepository<Payment> paymentRepository,
        IRepository<Order> orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;

        _accessToken = configuration["MercadoPago:AccessToken"]
            ?? throw new ArgumentNullException("MercadoPago:AccessToken not configured");

        _notificationUrl = configuration["MercadoPago:NotificationUrl"]
            ?? throw new ArgumentNullException("MercadoPago:NotificationUrl not configured");

        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<PaymentResponse> CreatePaymentPreferenceAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating payment preference for Order {OrderId}", request.OrderId);

        var order = await _orderRepository.GetById(request.OrderId, "Customer")
            ?? throw new OrderNotFoundException(request.OrderId);

        if (order.Items.Count == 0)
            throw new InvalidOperationException("Cannot create payment for order without items");

        // Verificar si ya existe un pago para esta orden
        var existingPayment = await _paymentRepository.First(
            p => p.OrderId == request.OrderId);

        if (existingPayment != null)
        {
            _logger.LogWarning("Payment already exists for Order {OrderId}", request.OrderId);
            throw new PaymentAlreadyExistsException(request.OrderId);
        }

        PaymentResponse response = null!;

        await _unitOfWork.ExecuteAsync(async () =>
        {
            var preferenceRequest = new PreferenceRequest
            {
                Items = order.Items.Select(item => new PreferenceItemRequest
                {
                    Id = item.ProductId.ToString(),
                    Title = item.ProductName,
                    Quantity = item.Quantity,
                    CurrencyId = "ARS",
                    UnitPrice = item.UnitPrice
                }).ToList(),
                ExternalReference = order.Id.ToString(),
                NotificationUrl = _notificationUrl,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = request.BackUrl ?? "http://localhost:5173/payment/success",
                    Failure = request.BackUrl ?? "http://localhost:5173/payment/failure",
                    Pending = request.BackUrl ?? "http://localhost:5173/payment/pending"
                },
                AutoReturn = "approved",
                Payer = new PreferencePayerRequest
                {
                    Email = order.Customer?.Email ?? "test@test.com"
                }
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(preferenceRequest, cancellationToken: cancellationToken);

            _logger.LogInformation("Mercado Pago preference created: {PreferenceId}", preference.Id);

            var payment = Payment.Create(
                order.Id,
                preference.Id,
                order.TotalAmount,
                order.Id.ToString());

            await _paymentRepository.Add(payment);

            response = new PaymentResponse
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                PreferenceId = payment.PreferenceId,
                InitPoint = preference.InitPoint,
                Amount = payment.Amount,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };

        }, cancellationToken);

        _logger.LogInformation("Payment created successfully for Order {OrderId}", request.OrderId);
        return response;
    }

    public async Task<PaymentDetailResponse> GetPaymentByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting payment for Order {OrderId}", orderId);

        var payment = await _paymentRepository.First(
            p => p.OrderId == orderId)
            ?? throw new PaymentNotFoundException(orderId);

        return _mapper.Map<PaymentDetailResponse>(payment);
    }

    public async Task<PaymentDetailResponse> GetPaymentByIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting payment {PaymentId}", paymentId);

        var payment = await _paymentRepository.GetById(paymentId)
            ?? throw new PaymentNotFoundException(paymentId);

        return _mapper.Map<PaymentDetailResponse>(payment);
    }

    public async Task ProcessWebhookNotificationAsync(
        string paymentId,
        string topic,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing webhook notification for Payment {PaymentId}, Topic: {Topic}",
            paymentId, topic);

        if (topic != "payment")
        {
            _logger.LogWarning("Ignoring notification with topic: {Topic}", topic);
            return;
        }

        try
        {
            var client = new MercadoPago.Client.Payment.PaymentClient();
            var mpPayment = await client.GetAsync(long.Parse(paymentId), cancellationToken: cancellationToken);

            if (mpPayment == null)
            {
                _logger.LogWarning("Payment not found in Mercado Pago: {PaymentId}", paymentId);
                return;
            }

            var externalReference = mpPayment.ExternalReference;
            if (string.IsNullOrEmpty(externalReference) || !Guid.TryParse(externalReference, out var orderId))
            {
                _logger.LogWarning("Invalid external reference in payment: {ExternalReference}", externalReference);
                return;
            }

            var payment = await _paymentRepository.First(
                p => p.OrderId == orderId);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found for Order {OrderId}", orderId);
                return;
            }

            await _unitOfWork.ExecuteAsync(async () =>
            {
                var status = MapMercadoPagoStatus(mpPayment.Status);
                var method = MapPaymentMethod(mpPayment.PaymentTypeId);

                payment.UpdateStatus(
                    paymentId,
                    status,
                    method,
                    mpPayment.Payer?.Email,
                    mpPayment.StatusDetail);

                await _paymentRepository.Update(payment);

                // Si el pago fue aprobado, actualizar el estado de la orden
                if (status == PaymentStatus.Approved)
                {
                    var order = await _orderRepository.GetById(orderId);
                    if (order != null && order.Status == OrderStatus.Pending)
                    {
                        order.MarkAsProcessing();
                        await _orderRepository.Update(order);
                        _logger.LogInformation("Order {OrderId} marked as Processing after payment approval", orderId);
                    }
                }

            }, cancellationToken);

            _logger.LogInformation("Payment {PaymentId} updated successfully with status {Status}",
                paymentId, mpPayment.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook notification for Payment {PaymentId}", paymentId);
            throw;
        }
    }

    private static PaymentStatus MapMercadoPagoStatus(string? status)
    {
        return status switch
        {
            "approved" => PaymentStatus.Approved,
            "rejected" => PaymentStatus.Rejected,
            "pending" => PaymentStatus.Pending,
            "in_process" => PaymentStatus.InProcess,
            "in_mediation" => PaymentStatus.InMediation,
            "cancelled" => PaymentStatus.Cancelled,
            "refunded" => PaymentStatus.Refunded,
            "charged_back" => PaymentStatus.ChargedBack,
            _ => PaymentStatus.Pending
        };
    }

    private static PaymentMethod? MapPaymentMethod(string? paymentType)
    {
        return paymentType switch
        {
            "credit_card" => PaymentMethod.CreditCard,
            "debit_card" => PaymentMethod.DebitCard,
            "bank_transfer" => PaymentMethod.BankTransfer,
            "ticket" => PaymentMethod.Cash,
            "account_money" => PaymentMethod.Other,
            _ => PaymentMethod.Other
        };
    }
}
