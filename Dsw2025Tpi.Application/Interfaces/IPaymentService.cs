using Dsw2025Tpi.Application.Dtos.Payments;

namespace Dsw2025Tpi.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse> CreatePaymentPreferenceAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<PaymentDetailResponse> GetPaymentByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    Task<PaymentDetailResponse> GetPaymentByIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default);

    Task ProcessWebhookNotificationAsync(
        string paymentId,
        string topic,
        CancellationToken cancellationToken = default);
}
