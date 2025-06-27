using AutoMapper;
using Dsw2025Tpi.Application.Dtos.Requests;
using Dsw2025Tpi.Application.Dtos.Responses;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces.Repositories;


namespace Dsw2025Tpi.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductResponse?> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetById(id);
            return product is null ? null : _mapper.Map<ProductResponse>(product);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAll() ?? new List<Product>();
            return products.Select(p => _mapper.Map<ProductResponse>(p));
        }

        public async Task<ProductResponse> CreateAsync(ProductRequest request)
        {
            // Validación de SKU único (regla de negocio)
            var existing = await _unitOfWork.Products.First(p => p.Sku == request.Sku);
            if (existing != null)
                throw new ProductAlreadyExistsException(request.Sku);

            var product = _mapper.Map<Product>(request);

            await _unitOfWork.Products.Add(product);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse?> UpdateAsync(Guid productId, ProductRequest updatedRequest)
        {
            var existing = await _unitOfWork.Products.GetById(productId);
            if (existing is null)
                return null;

            // Validación de SKU único si se modifica el SKU
            if (!string.Equals(existing.Sku, updatedRequest.Sku, StringComparison.OrdinalIgnoreCase))
            {
                var other = await _unitOfWork.Products.First(p => p.Sku == updatedRequest.Sku);
                if (other != null)
                    throw new ProductAlreadyExistsException(updatedRequest.Sku);
            }

            // Mapear los campos actualizables desde el DTO
            existing.Name = updatedRequest.Name;
            existing.Description = updatedRequest.Description;
            existing.CurrentUnitPrice = updatedRequest.CurrentUnitPrice;
            existing.StockQuantity = updatedRequest.StockQuantity;
            existing.Sku = updatedRequest.Sku;

            await _unitOfWork.Products.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProductResponse>(existing);
        }

        public async Task DisableAsync(Guid productId)
        {
            var product = await _unitOfWork.Products.GetById(productId);

            if (product is null)
                throw new NotFoundException("Producto no encontrado.");

            if (!product.IsActive)
                throw new ApplicationException("El producto ya está inhabilitado.");

            product.IsActive = false;
            await _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
