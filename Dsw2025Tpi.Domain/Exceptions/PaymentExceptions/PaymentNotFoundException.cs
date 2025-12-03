using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.PaymentExceptions;

public sealed class PaymentNotFoundException : ExceptionBase
{
    public Guid PaymentId { get; }

    public PaymentNotFoundException(Guid paymentId)
        : base("PAYMENT_NOT_FOUND")
    {
        PaymentId = paymentId;
    }
}
