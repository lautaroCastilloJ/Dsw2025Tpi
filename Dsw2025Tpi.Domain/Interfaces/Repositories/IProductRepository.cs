using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{

}


/*
 Hacer una interfaz repository especializada para cada entidad es recomendable
cuando querés dejar abierta la posibilidad de métodos específicos para productos
en el futuro (por ejemplo: búsquedas por SKU, obtener productos activos, etc.), 
pero sin repetir la lógica genérica de CRUD.

¿Qué lográs con esto?
IProductRepository hereda de IRepository<Product>, así que ya tiene todos los métodos genéricos 
(GetById, GetAll, Add, Update, Delete, etc.).

Si más adelante necesitás métodos propios para productos, podés agregarlos solo en esa interfaz, por ejemplo:

Task<Product?> GetBySkuAsync(string sku);
Task<IEnumerable<Product>> GetActiveProductsAsync();

Facilita el principio SOLID de "interfaz segregada" y la inyección de dependencias más específica.

¿Cómo implementarías el repositorio concreto?
Por ejemplo:

public class ProductRepository : EfRepository<Product>, IProductRepository
    {
        public ProductRepository(Dsw2025TpiContext context) : base(context) { }

        // Aquí puedes agregar métodos personalizados para Product si los necesitás.
    }

Resumen:
Vas por el camino correcto, la estructura está bien y te permite crecer el proyecto de forma ordenada.
¿Querés agregar un método personalizado de ejemplo o seguimos con otro repositorio/entidad?
 
 */