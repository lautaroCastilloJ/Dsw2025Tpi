using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderItemQuantityException : ExceptionBase
{
    public InvalidOrderItemQuantityException()
        : base("ORDERITEM_INVALID_QUANTITY") { }
}
