namespace Dsw2025Tpi.Application.Exceptions;

public class InsufficientStockException : Exception
{
    public InsufficientStockException(string productName, string sku)
        : base($"Stock insuficiente para el producto {productName} (SKU: {sku}).")
    {
    }
}
