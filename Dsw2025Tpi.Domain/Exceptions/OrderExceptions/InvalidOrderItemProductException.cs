using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderItemProductException : ExceptionBase
{
    public InvalidOrderItemProductException()
        : base("ORDERITEM_INVALID_PRODUCT") { }
}
