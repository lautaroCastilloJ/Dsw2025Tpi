using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;

namespace Dsw2025Tpi.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(OrderRequest request);
}
