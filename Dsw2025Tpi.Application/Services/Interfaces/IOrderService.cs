using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;

namespace Dsw2025Tpi.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<OrderResponse>> GetAllAsync(
            string? status = null,
            Guid? customerId = null,
            int pageNumber = 1,
            int pageSize = 10
        );
        Task<OrderResponse> CreateAsync(OrderRequest request);
        Task<OrderResponse?> UpdateStatusAsync(Guid orderId, string newStatus);
    }
}
