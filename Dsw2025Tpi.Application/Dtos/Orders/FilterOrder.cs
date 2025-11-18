namespace Dsw2025Tpi.Application.Dtos.Orders;

public sealed record FilterOrder(
    string? Status,
    Guid? CustomerId,
    int? PageNumber,
    int? PageSize
);
