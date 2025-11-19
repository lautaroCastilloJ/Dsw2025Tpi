using Dsw2025Tpi.Domain.Exceptions.Common;

namespace Dsw2025Tpi.Domain.Exceptions.ProductExceptions;

public sealed class ProductAlreadyExistsException : ExceptionBase
{
    public string Field { get; }
    public string Value { get; }
    public Guid ExistingProductId { get; }

    public ProductAlreadyExistsException(string field, string value, Guid existingProductId)
        : base("PRODUCT_ALREADY_EXISTS", $"Ya existe un producto con {field} '{value}'.")
    {
        Field = field;
        Value = value;
        ExistingProductId = existingProductId;
    }

    public ProductAlreadyExistsException(
        string field,
        string value,
        Guid existingProductId,
        string message)
        : base("PRODUCT_ALREADY_EXISTS", message)
    {
        Field = field;
        Value = value;
        ExistingProductId = existingProductId;
    }
}
