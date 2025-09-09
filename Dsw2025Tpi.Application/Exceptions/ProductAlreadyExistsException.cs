using System;

namespace Dsw2025Tpi.Application.Exceptions;

public class ProductAlreadyExistsException : Exception
{
    public Guid? ExistingProductId { get; }

    public ProductAlreadyExistsException(string field, string value, Guid? existingProductId = null, string? message = null)
        : base(message ?? $"Ya existe un producto con el mismo {field}: '{value}'.")
    {
        ExistingProductId = existingProductId;
    }
}
