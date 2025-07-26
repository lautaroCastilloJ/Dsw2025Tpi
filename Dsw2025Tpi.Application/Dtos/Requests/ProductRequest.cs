namespace Dsw2025Tpi.Application.Dtos.Requests;

public record ProductRequest(
    string Sku,
    string InternalCode,
    string Name,
    string? Description,
    decimal CurrentUnitPrice,
    int StockQuantity
);

