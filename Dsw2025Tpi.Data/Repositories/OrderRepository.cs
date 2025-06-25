using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces.Repositories;

namespace Dsw2025Tpi.Data.Repositories;

public class OrderRepository : EfRepository<Order>, IOrderRepository
{
    public OrderRepository(Dsw2025TpiContext context)
        : base(context)
    {
    }

    // Métodos específicos para Order pueden agregarse aquí más adelante
}
