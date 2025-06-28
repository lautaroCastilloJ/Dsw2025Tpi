using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Entities;

public class Product : EntityBase
{
    public string Sku { get; private set; } = default!;
    public string InternalCode { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal CurrentUnitPrice { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Product() { } // For EF Core

    public static Product Create(string sku, string internalCode, string name, string? description, decimal price, int stockQty)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU obligatorio.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre obligatorio.");
        if (price <= 0) throw new ArgumentException("Precio debe ser mayor a cero.");
        if (stockQty < 0) throw new ArgumentException("Stock no puede ser negativo.");

        return new Product
        {
            Sku = sku,
            InternalCode = internalCode,
            Name = name,
            Description = description,
            CurrentUnitPrice = price,
            StockQuantity = stockQty
        };
    }

    public void UpdateSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU inválido.");
        Sku = sku;
    }

    public void UpdateInternalCode(string internalCode)
    {
        InternalCode = internalCode;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre inválido.");
        Name = name;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdatePrice(decimal price)
    {
        if (price <= 0) throw new ArgumentException("El precio debe ser mayor a cero.");
        CurrentUnitPrice = price;
    }

    public void UpdateStock(int stockQuantity)
    {
        if (stockQuantity < 0) throw new ArgumentException("Stock no puede ser negativo.");
        StockQuantity = stockQuantity;
    }

    public void Disable()
    {
        IsActive = false;
    }

    public void Enable()
    {
        IsActive = true;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Cantidad inválida.");
        if (quantity > StockQuantity) throw new InvalidOperationException("Stock insuficiente.");
        StockQuantity -= quantity;
    }

    public bool HasStock(int quantity) => quantity <= StockQuantity;
}
