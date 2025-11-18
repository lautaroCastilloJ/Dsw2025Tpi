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
        var query = _productRepository.GetAllQueryable();

        // Filtro por estado
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var status = filter.Status.Trim().ToLowerInvariant();
            if (status is "active" or "true")
                query = query.Where(p => p.IsActive);
            else if (status is "inactive" or "false")
                query = query.Where(p => !p.IsActive);
        }

        // Búsqueda por texto (SKU / Name)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(p =>
                p.Sku.Contains(term) ||
                p.Name.Contains(term));
        }

        // Normalizar paginación
        var pageNumber = filter.PageNumber.HasValue && filter.PageNumber > 0 ? filter.PageNumber.Value : 1;
        var pageSize = filter.PageSize.HasValue && filter.PageSize > 0 ? filter.PageSize.Value : 10;
        if (pageSize > 100) pageSize = 100;

        var totalCount = query.Count();

        var items = query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtoItems = items
            .Select(p => _mapper.Map<ProductListItemDto>(p))
            .ToList();

        var result = PagedResult<ProductListItemDto>.Create(
            dtoItems,
            totalCount,
            pageNumber,
            pageSize);

        return Task.FromResult(result); // No es async porque todo se ejecuta en memoria
    }
}
