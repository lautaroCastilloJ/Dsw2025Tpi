using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.AuthExceptions;

public sealed class InvalidCredentialsException : ExceptionBase
{
    public InvalidCredentialsException()
        : base("AUTH_INVALID_CREDENTIALS") { }
}
