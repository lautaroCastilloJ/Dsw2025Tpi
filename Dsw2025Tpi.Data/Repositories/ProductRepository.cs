using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces.Repositories;

namespace Dsw2025Tpi.Data.Repositories;

public class ProductRepository : EfRepository<Product>, IProductRepository
{
    public ProductRepository(Dsw2025TpiContext context)
        : base(context)
    {
    }

    // Aquí puedes agregar métodos específicos para Product si necesitas
}
