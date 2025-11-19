using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderStatusTransitionException : ExceptionBase
{
    public OrderStatus FromStatus { get; }
    public OrderStatus ToStatus { get; }

    public InvalidOrderStatusTransitionException(OrderStatus from, OrderStatus to)
        : base("ORDER_INVALID_STATUS_TRANSITION", 
            $"No se puede cambiar el estado de la orden de '{from}' a '{to}'. " +
            GetAllowedTransitionsMessage(from))
    {
        FromStatus = from;
        ToStatus = to;
    }

    private static string GetAllowedTransitionsMessage(OrderStatus currentStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => "Transiciones permitidas: Processing, Cancelled.",
            OrderStatus.Processing => "Transiciones permitidas: Shipped, Cancelled.",
            OrderStatus.Shipped => "Transiciones permitidas: Delivered, Cancelled.",
            OrderStatus.Delivered => "Esta orden ya está entregada y no puede cambiar de estado.",
            OrderStatus.Cancelled => "Esta orden está cancelada y no puede cambiar de estado.",
            _ => "No hay transiciones permitidas desde este estado."
        };
    }
}