using POS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> GetProductByIdAsync(int id);
        Task CreateProductAsync(ProductDTO dto);
        Task UpdateProductAsync(ProductDTO dto);
        Task DeleteProductAsync(int id);
        Task<bool> UpdateStockAsync(int productId, decimal quantity);
        Task<ProductDTO> GetProductByBarcodeAsync(string barcode);
        Task<IEnumerable<ProductDTO>> GetLowStockProductsAsync();
    }
}
