using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderItemPriceException : ExceptionBase
{
    public InvalidOrderItemPriceException()
        : base("ORDERITEM_INVALID_PRICE") { }
}