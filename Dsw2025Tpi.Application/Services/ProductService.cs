using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Products;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Pagination;
using Dsw2025Tpi.Domain.Entities;
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
        // El MappingProfile ya usa Product.Create(...) para respetar el dominio
        var product = _mapper.Map<Product>(request);

        await _productRepository.Add(product);

        // Devuelves el producto creado como respuesta
        return _mapper.Map<ProductResponse>(product);
    }

    // 2) Obtener todos los productos → GET /api/products
    //    El controller decidirá si devuelve 200 OK o 204 NoContent
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAll();

        if (products is null || !products.Any())
            return Enumerable.Empty<ProductResponse>();

        return products.Select(p => _mapper.Map<ProductResponse>(p))
                       .ToList();
    }

    // 3) Obtener producto por Id → GET /api/products/{id}
    //    El controller mapea null → 404 NotFound
    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetById(id);

        if (product is null)
            return null;

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
        // 1) Query base
        var query = _productRepository.GetAllQueryable();

        // 2) Filtro por estado (activo / inactivo)
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

        // 3) Filtro de búsqueda (SKU / Nombre)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();

            query = query.Where(p =>
                p.Sku.Contains(term) ||
                p.Name.Contains(term));
        }

        // 4) Normalizar paginación
        int pageNumber = (filter.PageNumber.HasValue && filter.PageNumber > 0) ? filter.PageNumber.Value : 1;
        int pageSize = (filter.PageSize.HasValue && filter.PageSize > 0) ? filter.PageSize.Value : 10;

        const int maxPageSize = 100;
        if (pageSize > maxPageSize)
            pageSize = maxPageSize;

        // 5) Total antes de paginar
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

        // 6) Paginación + proyección a DTO liviano
        var items = query
            .OrderBy(p => p.Sku) // criterio estable
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                CurrentUnitPrice = p.CurrentUnitPrice,
                IsActive = p.IsActive
            })
            .ToList();

        var result = PagedResult<ProductListItemDto>.Create(
            items,
            totalCount,
            pageNumber,
            pageSize);

        return Task.FromResult(result);
    }
}


