using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.AuthExceptions;

public sealed class UserCreationFailedException : ExceptionBase
{
    public UserCreationFailedException(string? errors = null)
        : base("AUTH_USER_CREATION_FAILED")
    {
        Errors = errors;
    }

    public string? Errors { get; }
}
