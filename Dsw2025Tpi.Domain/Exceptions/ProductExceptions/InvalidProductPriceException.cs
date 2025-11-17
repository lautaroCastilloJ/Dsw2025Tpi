using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class InvalidProductPriceException : ExceptionBase
{
    public InvalidProductPriceException()
        : base("PRODUCT_INVALID_PRICE") { }
}
