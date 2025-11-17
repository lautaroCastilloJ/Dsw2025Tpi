using Dsw2025Tpi.Domain.Exceptions.Common;


namespace Dsw2025Tpi.Domain.Exceptions.CustomerExceptions;

public sealed class CustomerAlreadyInactiveException : ExceptionBase
{
    public Guid CustomerId { get; }

    public CustomerAlreadyInactiveException(Guid id)
        : base("CUSTOMER_ALREADY_INACTIVE")
    {
        CustomerId = id;
    }
}

