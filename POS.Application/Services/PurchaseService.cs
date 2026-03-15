using POS.Application.DTOs;
using POS.Application.ViewModels;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        public async Task<IEnumerable<Purchase>> GetAllPurchasesAsync()
        {
            return await _unitOfWork.Purchases.GetAllAsync();
        }
        public async Task ProcessPurchaseAsync(PurchaseViewModel model)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var purchase = new Purchase
                {
                    SupplierId = model.SupplierId,
                    PurchaseDate = model.PurchaseDate,
                    TotalAmount = model.Items.Sum(i => i.Quantity * i.UnitCost),
                    PurchaseItems = new List<PurchaseItem>()
                };

                foreach (var item in model.Items)
                {
                    purchase.PurchaseItems.Add(new PurchaseItem
                    {
                        ProductId = item.ProductId,
                        Quantity = (int)item.Quantity,
                        UnitCost = item.UnitCost
                    });

                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity; 
                        product.Cost = item.UnitCost;
                        _unitOfWork.Products.Update(product);
                    }
                }

                await _unitOfWork.Purchases.AddAsync(purchase);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<ProductDTO>> GetLowStockProductsAsync()
        {
            var allProducts = await _unitOfWork.Products.GetAllAsync();
            var lowStockProducts = allProducts.Where(p => p.StockQuantity <= p.ReorderLevel);
            return lowStockProducts.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Barcode = p.Barcode,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel
            }).ToList();
        }
    }
}
