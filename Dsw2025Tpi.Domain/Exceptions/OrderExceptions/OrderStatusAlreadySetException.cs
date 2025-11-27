using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.Common;


namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class OrderStatusAlreadySetException : ExceptionBase
{
    public OrderStatus Status { get; }

    public OrderStatusAlreadySetException(OrderStatus status)
        : base("ORDER_STATUS_ALREADY_SET")
    {
        Status = status;
    }
}


