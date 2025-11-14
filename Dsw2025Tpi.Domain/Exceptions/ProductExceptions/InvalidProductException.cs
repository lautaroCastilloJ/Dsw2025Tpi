using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class InvalidProductException : ExceptionBase
{
    public InvalidProductException(string code)
        : base(code)
    { }
}