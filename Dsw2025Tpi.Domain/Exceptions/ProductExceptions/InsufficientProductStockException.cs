using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class InsufficientProductStockException : ExceptionBase
{
    public InsufficientProductStockException()
        : base("PRODUCT_INSUFFICIENT_STOCK") { }
}
