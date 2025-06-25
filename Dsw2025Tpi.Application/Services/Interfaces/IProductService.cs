using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;

namespace Dsw2025Tpi.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductResponse>> GetAllAsync();
        Task<ProductResponse> CreateAsync(ProductRequest request);
        Task<ProductResponse?> UpdateAsync(Guid productId, ProductRequest request);
        Task DisableAsync(Guid productId); // Inhabilitar el producto (soft delete)
    }
}
