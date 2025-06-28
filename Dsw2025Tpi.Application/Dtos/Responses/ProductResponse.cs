namespace Dsw2025Tpi.Application.Dtos.Responses;

public record ProductResponse(
    Guid Id,
    string Sku,
    string InternalCode,
    string Name,
    string? Description,
    decimal CurrentUnitPrice,
    int StockQuantity,
    bool IsActive
);
