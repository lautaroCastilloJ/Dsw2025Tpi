using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;


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
        var product = await _productRepository.GetById(id);
        if (product.IsActive == false)
            throw new ArgumentException($"El producto con ID {id} no está activo o no existe.");
        return product is null ? null : _mapper.Map<ProductResponse>(product);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAll();
        var activeProducts = products!.Where(p => p.IsActive); // Solo los activos y uso ! porque es posible que no haya ningun activo o sea null
        
        if (!activeProducts.Any())
            throw new ArgumentException("No hay productos activos disponibles.");

        return _mapper.Map<IEnumerable<ProductResponse>>(activeProducts);
    }


    public async Task<ProductResponse> CreateAsync(ProductRequest request)
    {
        var skuLower = request.Sku.ToLower();
        var codeLower = request.InternalCode.ToLower();

        // Buscar productos que coincidan por SKU o InternalCode, activos o inactivos
        var existing = await _productRepository.First(p =>
            p.Sku.ToLower() == skuLower || p.InternalCode.ToLower() == codeLower);

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
                    $"Ya existe un producto con {field} '{value}', pero está dado de baja. Podés modificar su SKU y/o InternalCode con el ID: {existing.Id}"
                );
            }

            throw new ProductAlreadyExistsException(field, value, existing.Id);
        }


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
