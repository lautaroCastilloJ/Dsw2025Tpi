using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Products;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Pagination;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Exceptions.ProductExceptions;
using Dsw2025Tpi.Domain.Interfaces;
using System.Linq;

namespace Dsw2025Tpi.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // 1) Crear producto  → POST /api/products
    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        var skuLower = request.Sku.Trim().ToLowerInvariant();
        var codeLower = request.InternalCode.Trim().ToLowerInvariant();

        // Buscar por SKU o InternalCode (activos o inactivos)
        var existing = await _productRepository.First(p =>
            p.Sku.ToLower() == skuLower ||
            p.InternalCode.ToLower() == codeLower);

        if (existing is not null)
        {
            var field = existing.Sku.Equals(request.Sku, StringComparison.OrdinalIgnoreCase)
                ? "SKU"
                : "InternalCode";

            var value = field == "SKU" ? request.Sku : request.InternalCode;

            if (!existing.IsActive)
            {
                throw new ProductAlreadyExistsException(
                    field,
                    value,
                    existing.Id,
                    $"Ya existe un producto con {field} '{value}', pero está deshabilitado. " +
                    $"Puedes modificar su SKU y/o InternalCode usando el Id {existing.Id}."
                );
            }

            // Producto activo con mismo SKU / código interno
            throw new ProductAlreadyExistsException(field, value, existing.Id);
        }

        // Si no existe, creamos el producto normalmente
        var product = Product.Create(
            request.Sku,
            request.InternalCode,
            request.Name,
            request.Description ?? string.Empty,
            request.CurrentUnitPrice,
            request.StockQuantity);

        await _productRepository.Add(product);

        return _mapper.Map<ProductResponse>(product);
    }


    // 2) Obtener todos los productos activos con paginación → GET /api/products
    public async Task<PagedResult<ProductResponse>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var query = _productRepository.GetAllQueryable();

        // Filtrar solo productos activos
        query = query.Where(p => p.IsActive);

        // Normalizar paginación
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        const int maxPageSize = 100;
        if (pageSize > maxPageSize)
            pageSize = maxPageSize;

        // Total antes de paginar
        var totalCount = query.Count();

        if (totalCount == 0)
        {
            return PagedResult<ProductResponse>.Create(
                new List<ProductResponse>(),
                0,
                pageNumber,
                pageSize);
        }

        // Paginación + proyección a DTO
        var items = query
            .OrderBy(p => p.Sku) // criterio estable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtoItems = items
            .Select(p => _mapper.Map<ProductResponse>(p))
            .ToList();

        return PagedResult<ProductResponse>.Create(
            dtoItems,
            totalCount,
            pageNumber,
            pageSize);
    }

    // 3) Obtener producto por Id → GET /api/products/{id}
    //    El controller mapea null → 404 NotFound
    public async Task<ProductResponse?> GetByIdAsync(Guid productId)
    {
        var product = await _productRepository.GetById(productId);

        if (product is null)
            throw new ProductNotFoundException(productId);
        if (!product.IsActive)
            throw new ProductInactiveException();

        return _mapper.Map<ProductResponse>(product);
    }

    // 4) Actualizar producto → PUT /api/products/{id}
    public async Task<ProductResponse?> UpdateAsync(Guid productId, ProductRequest request)
    {
        var product = await _productRepository.GetById(productId);

        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.UpdateDetails(
            request.Sku,
            request.InternalCode,
            request.Name,
            request.Description ?? string.Empty,
            request.CurrentUnitPrice,
            request.StockQuantity);

        await _productRepository.Update(product);

        return _mapper.Map<ProductResponse>(product);
    }

    // 5) Inhabilitar producto → PATCH /api/products/{id}
    public async Task DisableAsync(Guid productId)
    {
        var product = await _productRepository.GetById(productId);

        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }

        product.Deactivate();

        await _productRepository.Update(product);
    }

    // 6) Paginación de productos → GET /api/products/paged?...
    public Task<PagedResult<ProductListItemDto>> GetPagedAsync(
           FilterProductRequest filter,
           CancellationToken cancellationToken = default)
    {
        // i) Query base
        var query = _productRepository.GetAllQueryable();

        // ii) Filtro por estado (activo / inactivo)
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var status = filter.Status.Trim().ToLowerInvariant();

            query = status switch
            {
                "enabled" or "active" or "true" => query.Where(p => p.IsActive),
                "disabled" or "inactive" or "false" => query.Where(p => !p.IsActive),
                _ => query // valor inválido: no filtramos
            };
        }

        // iii) Filtro de búsqueda (SKU / Nombre)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();

            query = query.Where(p =>
                p.Sku.Contains(term) ||
                p.Name.Contains(term));
        }

        // iv) Normalizar paginación
        int pageNumber = (filter.PageNumber.HasValue && filter.PageNumber > 0) ? filter.PageNumber.Value : 1;
        int pageSize = (filter.PageSize.HasValue && filter.PageSize > 0) ? filter.PageSize.Value : 10;

        const int maxPageSize = 100;
        if (pageSize > maxPageSize)
            pageSize = maxPageSize;

        // v) Total antes de paginar
        var totalCount = query.Count();

        if (totalCount == 0)
        {
            var empty = PagedResult<ProductListItemDto>.Create(
                Array.Empty<ProductListItemDto>(),
                0,
                pageNumber,
                pageSize);

            return Task.FromResult(empty);
        }

        // vi) Paginación + proyección a DTO liviano
        var items = query
            .OrderBy(p => p.Sku) // criterio estable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemDto(
                p.Id,
                p.Sku,
                p.Name,
                p.CurrentUnitPrice,
                p.IsActive))
            .ToList();

        var result = PagedResult<ProductListItemDto>.Create(
            items,
            totalCount,
            pageNumber,
            pageSize);

        return Task.FromResult(result);
    }
}


