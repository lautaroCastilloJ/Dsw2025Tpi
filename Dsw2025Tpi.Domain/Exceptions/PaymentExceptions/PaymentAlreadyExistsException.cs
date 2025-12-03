using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.PaymentExceptions;

public sealed class PaymentAlreadyExistsException : ExceptionBase
{
    public Guid OrderId { get; }

    public PaymentAlreadyExistsException(Guid orderId)
        : base("PAYMENT_ALREADY_EXISTS")
    {
        OrderId = orderId;
    }
}
