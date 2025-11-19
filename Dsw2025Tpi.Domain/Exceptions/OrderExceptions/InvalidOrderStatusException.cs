using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderStatusException : ExceptionBase
{
    public string Status { get; }

    public InvalidOrderStatusException(string status)
        : base("ORDER_INVALID_STATUS", 
            $"El estado '{status}' no es válido. Estados válidos: Pending, Processing, Shipped, Delivered, Cancelled.")
    {
        Status = status;
    }
}
