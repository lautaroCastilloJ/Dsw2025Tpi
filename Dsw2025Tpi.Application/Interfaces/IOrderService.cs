using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using System.Security.Claims;

namespace Dsw2025Tpi.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(OrderRequest request);
    Task<IEnumerable<OrderResponse>> GetAllAsync(string? status, Guid? customerId, int pageNumber, int pageSize);
    Task<OrderResponse> GetByIdAsync(Guid id);
    Task<OrderResponse> UpdateStatusAsync(Guid id, string newStatus);




}
