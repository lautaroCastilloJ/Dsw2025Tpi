namespace Dsw2025Tpi.Application.Dtos.Requests;

public record ProductRequest(
    string Sku,
    string Name,
    string InternalCode,
    string Description,
    decimal CurrentUnitPrice,
    int StockQuantity,
    bool IsActive = true
);
