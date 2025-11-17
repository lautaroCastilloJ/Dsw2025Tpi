using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderStatusTransitionException : ExceptionBase
{
    public InvalidOrderStatusTransitionException(OrderStatus from, OrderStatus to)
        : base("ORDER_INVALID_STATUS_TRANSITION")
    { }
}