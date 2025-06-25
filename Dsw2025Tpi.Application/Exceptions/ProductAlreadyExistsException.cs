namespace Dsw2025Tpi.Application.Exceptions;

public class ProductAlreadyExistsException : Exception
{
    public ProductAlreadyExistsException(string sku)
        : base($"Ya existe un producto con el SKU '{sku}'.")
    {
    }
}
