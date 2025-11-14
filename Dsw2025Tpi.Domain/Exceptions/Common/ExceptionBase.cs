using Dsw2025Tpi.Domain.Resources;

namespace Dsw2025Tpi.Domain.Exceptions.Common;

public abstract class ExceptionBase : Exception
{
    public string Code { get; }

    protected ExceptionBase(string code)
        : base(GetMessageFromResource(code))
    {
        Code = code;
    }

    protected ExceptionBase(string code, Exception innerException)
        : base(GetMessageFromResource(code), innerException)
    {
        Code = code;
    }

    private static string GetMessageFromResource(string code)
    {
        // Convertir dots "PRODUCT.INVALID_PRICE" a underscore si tu resx lo requiere
        // Soporta dos formatos:
        // - "PRODUCT.INVALID_PRICE"
        // - "PRODUCT_INVALID_PRICE"
        string key = code.Replace(".", "_");

        return ErrorMessages.ResourceManager.GetString(key)
            ?? $"Unknown error code: {code}";
    }
}

