using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.AuthExceptions;

public sealed class EmailAlreadyExistsException : ExceptionBase
{
    public string Email { get; }

    public EmailAlreadyExistsException(string email)
        : base("AUTH_EMAIL_ALREADY_EXISTS")
    {
        Email = email;
    }
}
