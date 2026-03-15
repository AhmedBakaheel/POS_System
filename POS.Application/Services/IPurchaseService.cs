using POS.Application.DTOs;
using POS.Application.ViewModels;
using POS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public interface IPurchaseService
    {
        Task<IEnumerable<Purchase>> GetAllPurchasesAsync();
        Task ProcessPurchaseAsync(PurchaseViewModel model);
        Task<IEnumerable<ProductDTO>> GetLowStockProductsAsync();
    }
}
