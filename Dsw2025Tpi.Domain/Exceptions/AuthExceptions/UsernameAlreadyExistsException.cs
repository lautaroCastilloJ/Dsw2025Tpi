using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.AuthExceptions;

public sealed class UsernameAlreadyExistsException : ExceptionBase
{
    public string Username { get; }

    public UsernameAlreadyExistsException(string username)
        : base("AUTH_USERNAME_ALREADY_EXISTS")
    {
        Username = username;
    }
}
