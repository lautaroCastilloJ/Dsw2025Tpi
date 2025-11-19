using Dsw2025Tpi.Application.Dtos.Products;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }


    // ---------------------------------------------------------------------------------------------------------------------
    // 1. Crear un producto  -> POST /api/products
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
    {
        var created = await _productService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }



    // ---------------------------------------------------------------------------------------------------------------------
    // 2. Obtener todos los productos (activos) con paginación
    //    GET /api/products?pageNumber=1&pageSize=10
    //    Público (sin autenticación)
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var paged = await _productService.GetAllAsync(pageNumber, pageSize);

        if (!paged.Items.Any())
            return NoContent();                // 204 si no hay productos

        return Ok(paged);                     // 200 con PagedResult<ProductResponse>
    }


    // ---------------------------------------------------------------------------------------------------------------------
    // 3. Obtener un producto por ID -> GET /api/products/{id}
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product is null)
            return NotFound();                // 404 si no existe

        return Ok(product);                   // 200
    }


    // ---------------------------------------------------------------------------------------------------------------------
    // 4. Actualizar un producto -> PUT /api/products/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequest request)
    {
        var updated = await _productService.UpdateAsync(id, request);

        if (updated is null)
            return NotFound();                // 404 si no existe

        return Ok(updated);                   // 200 con producto actualizado
    }


    // ---------------------------------------------------------------------------------------------------------------------
    // 5. Inhabilitar un producto -> PATCH /api/products/{id}
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Disable(Guid id)
    {
        await _productService.DisableAsync(id);
        // Si el servicio lanza ProductNotFoundException, tu middleware la convertirá en 404
        return NoContent();                   // 204 si fue exitoso
    }


    // ---------------------------------------------------------------------------------------------------------------------
    // 6. Listado paginado con filtro SOLO para administrador
    //    GET /api/products/admin?status=enabled&search=abc&pageNumber=1&pageSize=10
    [HttpGet("admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetPagedForAdmin(
        [FromQuery] FilterProductRequest filter,
        CancellationToken cancellationToken)
    {
        var paged = await _productService.GetPagedAsync(filter, cancellationToken);

        if (!paged.Items.Any())
            return NoContent();               // 204

        return Ok(paged);                     // 200 con PagedResult<ProductListItemDto>
    }
}
