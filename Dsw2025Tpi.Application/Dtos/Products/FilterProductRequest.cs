

namespace Dsw2025Tpi.Application.Dtos.Product;

public record FilterProductRequest(
    string? Status,
    string? Search,
    int? PageNumber,
    int? PageSize
);
