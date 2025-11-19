namespace Dsw2025Tpi.Application.Dtos.Orders;

public sealed record FilterOrder(
    string? Status,
    Guid? CustomerId,
    string? Search,
    int? PageNumber,
    int? PageSize
);
