using Dsw2025Tpi.Domain.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly Dsw2025TpiContext _context;

    // Propiedades de los repositorios
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    // Constructor con inyección de dependencias de los repositorios y el contexto
    public UnitOfWork(
        Dsw2025TpiContext context,
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _context = context;
        Products = productRepository;
        Orders = orderRepository;
    }

    // Guarda todos los cambios pendientes en la base de datos
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}


/*UnitOfWork es un patron de diseño utilizado con el proposito de asegurarse que cuando hay multiples repositorios 
 * 
 * que necesitan ser invocados o procesados para una petición, estas compartan una misma conexión a la base de datos.

De esta manera podemos reducir la cantidad de veces que una conexión a la base es creada para una transacción cuando esta 
utiliza diferentes repositorios.

Asimismo en esta clase tenemos metodos generales para el manejo del contexto de datos, como el SaveChangesAsync, 
que será el encargado de guardar los cambios realizados.*/