using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class ProductInactiveException : ExceptionBase
{
    public ProductInactiveException()
        : base("PRODUCT_INACTIVE") { }
}
