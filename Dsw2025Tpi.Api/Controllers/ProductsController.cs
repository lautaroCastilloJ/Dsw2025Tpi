using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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


    
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
    {
        var created = await _productService.CreateAsync(request);
        if (created is null)
            return BadRequest(new { error = "Error al crear el producto. Verifique los datos." });

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); // 201 Created
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllActiveProducts()
    {
        var products = await _productService.GetAllAsync();
        if (!products.Any())
            return NoContent(); // 204 si no hay productos

        return Ok(products); // 200 con la lista
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound(new { error = "Producto no encontrado." });

        return Ok(product); // 200
    }


 
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductRequest request)
    {
        var updated = await _productService.UpdateAsync(id, request);
        if (updated is null)
            return NotFound(new { error = "Producto no encontrado para actualizar." });

        return Ok(updated); // 200 OK
    }


    [HttpPatch("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Disable(Guid id)
    {
        var existing = await _productService.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new { error = "Producto no encontrado para inhabilitar." });

        await _productService.DisableAsync(id);
        return NoContent(); // 204
    }
}
