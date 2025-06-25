using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Domain.Interfaces.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    // Métodos específicos de Order, si los necesitás más adelante
    // Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
}
