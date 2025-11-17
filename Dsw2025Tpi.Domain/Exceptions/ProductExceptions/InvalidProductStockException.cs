using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class InvalidProductStockException : ExceptionBase
{
    public InvalidProductStockException()
        : base("PRODUCT_INVALID_STOCK") { }
}
