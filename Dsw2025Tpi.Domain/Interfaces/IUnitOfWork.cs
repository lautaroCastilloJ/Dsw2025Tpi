using Microsoft.Win32;
using System.Drawing;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    #region
    //Sirve para guardar todos los cambios pendientes en la base de datos de forma asíncrona, 
    //devolviendo la cantidad de registros afectados.
    #endregion

    // Exponer los repositorios que necesites manejar en la implementacion UnitOfWork:
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    
}


/*El UnitOfWork coordina y agrupa los cambios de varios repositorios para que se guarden en la base de datos en una única transacción.
En tu proyecto, te ayuda a mantener la consistencia de datos (por ejemplo, al crear una orden y actualizar el stock de productos, 
ambos cambios se guardan juntos o ninguno), y hace el código más organizado y fácil de mantener.

Una transacción es un proceso que agrupa varias operaciones en la base de datos para que todas se realicen juntas o ninguna se realice.
Así, si ocurre un error en medio del proceso, la base de datos vuelve al estado inicial y no quedan datos a medias ni inconsistentes.
 
 */