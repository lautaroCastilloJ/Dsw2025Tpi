using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetById(id); // validar si el producto IsActive
        return product is null ? null : _mapper.Map<ProductResponse>(product);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAll();
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        var product = Product.Create(
            request.Sku,
            request.InternalCode,
            request.Name,
            request.Description,
            request.CurrentUnitPrice,
            request.StockQuantity
        );

        await _productRepository.Add(product);
        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse?> UpdateAsync(Guid productId, ProductRequest request)
    {
        var existing = await _productRepository.GetById(productId);
        if (existing is null) return null;

        existing.UpdateSku(request.Sku);
        existing.UpdateInternalCode(request.InternalCode);
        existing.UpdateName(request.Name);
        existing.UpdateDescription(request.Description);
        existing.UpdatePrice(request.CurrentUnitPrice);
        existing.UpdateStock(request.StockQuantity);

        await _productRepository.Update(existing);
        return _mapper.Map<ProductResponse>(existing);
    }

    public async Task DisableAsync(Guid productId)
    {
        var existing = await _productRepository.GetById(productId);
        if (existing is null) return;

        existing.Disable();
        await _productRepository.Update(existing);
    }
}
