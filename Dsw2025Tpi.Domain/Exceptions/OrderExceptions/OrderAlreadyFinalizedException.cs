using Dsw2025Tpi.Domain.Enums;
using Dsw2025Tpi.Domain.Exceptions.Common;


namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class OrderAlreadyFinalizedException : ExceptionBase
{
    public Guid OrderId { get; }
    public OrderStatus CurrentStatus { get; }
    public OrderStatus RequestedStatus { get; }

    public OrderAlreadyFinalizedException(Guid orderId, OrderStatus currentStatus, OrderStatus requestedStatus)
        : base("ORDER_ALREADY_FINALIZED", 
            $"No se puede actualizar la orden porque ya está en estado '{currentStatus}'. " +
            $"Las órdenes en estado '{currentStatus}' no pueden cambiar a '{requestedStatus}' porque ya están finalizadas.")
    {
        OrderId = orderId;
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }
}