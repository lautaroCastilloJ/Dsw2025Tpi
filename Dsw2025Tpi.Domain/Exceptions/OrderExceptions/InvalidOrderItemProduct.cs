using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderItemProduct : ExceptionBase
{
    public InvalidOrderItemProduct()
        : base("ORDERITEM_INVALID_PRODUCT") { }
}
