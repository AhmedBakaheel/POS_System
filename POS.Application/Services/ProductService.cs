using AutoMapper;
using POS.Application.DTOs;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Application.Services
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

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task CreateProductAsync(ProductDTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateProductAsync(ProductDTO dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
            if (product != null)
            {
                _mapper.Map(dto, product);
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, decimal quantity)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return false;

            product.StockQuantity += quantity;
            _unitOfWork.Products.Update(product);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<ProductDTO> GetProductByBarcodeAsync(string barcode)
        {
            var product = await _unitOfWork.Products.GetByBarcodeAsync(barcode);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetLowStockProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var lowStockItems = products.Where(p => p.StockQuantity <= p.ReorderLevel);
            return _mapper.Map<IEnumerable<ProductDTO>>(lowStockItems);
        }
    }
}