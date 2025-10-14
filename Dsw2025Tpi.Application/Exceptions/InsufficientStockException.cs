namespace Dsw2025Tpi.Application.Exceptions;

public class InsufficientStockException : ExceptionBase
{
    public InsufficientStockException(string code, string message) : base(code, message)
    {

    }
}
