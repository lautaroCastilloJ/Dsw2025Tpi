using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class NoProductsAvailableException : ExceptionBase
{
    public NoProductsAvailableException()
        : base("NO_PRODUCTS_AVAILABLE") { }
}
