using Dsw2025Tpi.Application.Dtos.Payments;
using Swashbuckle.AspNetCore.Filters;

namespace Dsw2025Tpi.Api.Examples;

public class CreatePaymentRequestExample : IExamplesProvider<CreatePaymentRequest>
{
    public CreatePaymentRequest GetExamples()
    {
        return new CreatePaymentRequest
        {
            OrderId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            BackUrl = "http://localhost:5173/payment/result"
        };
    }
}

public class PaymentResponseExample : IExamplesProvider<PaymentResponse>
{
    public PaymentResponse GetExamples()
    {
        return new PaymentResponse
        {
            Id = Guid.Parse("8b3c7a42-1234-4567-89ab-cdef01234567"),
            OrderId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            PreferenceId = "123456789-abcd-1234-5678-90abcdef1234",
            InitPoint = "https://www.mercadopago.com.ar/checkout/v1/redirect?pref_id=123456789-abcd-1234-5678-90abcdef1234",
            Amount = 1500.00m,
            Status = Domain.Enums.PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class PaymentDetailResponseExample : IExamplesProvider<PaymentDetailResponse>
{
    public PaymentDetailResponse GetExamples()
    {
        return new PaymentDetailResponse
        {
            Id = Guid.Parse("8b3c7a42-1234-4567-89ab-cdef01234567"),
            OrderId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            MercadoPagoId = "1234567890",
            PreferenceId = "123456789-abcd-1234-5678-90abcdef1234",
            Amount = 1500.00m,
            Status = Domain.Enums.PaymentStatus.Approved,
            Method = Domain.Enums.PaymentMethod.CreditCard,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            ProcessedAt = DateTime.UtcNow.AddMinutes(-5),
            PayerEmail = "customer@example.com",
            StatusDetail = "accredited"
        };
    }
}
