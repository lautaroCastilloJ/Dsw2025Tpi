using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class OrderWithoutItemsException : ExceptionBase
{
    public OrderWithoutItemsException()
        : base("ORDER_WITHOUT_ITEMS") { }
}
