using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.OrderExceptions;

public sealed class InvalidOrderCustomerException : ExceptionBase
{
    public InvalidOrderCustomerException()
        : base("ORDER_INVALID_CUSTOMER") { }
}
