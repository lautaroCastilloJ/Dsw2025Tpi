using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class InvalidProductQuantityException : ExceptionBase
{
    public InvalidProductQuantityException()
        : base("PRODUCT_INVALID_QUANTITY") { }
}
