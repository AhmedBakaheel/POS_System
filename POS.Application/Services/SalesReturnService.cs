using POS.Application.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public class SalesReturnService : ISalesReturnService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesReturnService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ProcessReturnAsync(SalesReturnDTO returnDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var salesReturn = new SalesReturn
                {
                    SaleId = returnDto.SaleId,
                    ReturnDate = DateTime.Now,
                    TotalAmount = returnDto.TotalAmount,
                    Reason = returnDto.Reason,
                    ShiftId = returnDto.ShiftId,
                    ReturnItems = new List<SalesReturnItem>()
                };

                foreach (var item in returnDto.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity; 
                        _unitOfWork.Products.Update(product);
                    }

                    salesReturn.ReturnItems.Add(new SalesReturnItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }
                var boxTransaction = new BoxTransaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = returnDto.TotalAmount,
                    Type = TransactionType.Expense,
                    Description = $"مرتجع مبيعات للفاتورة رقم #{returnDto.SaleId}",
                    ShiftId = returnDto.ShiftId
                };

                await _unitOfWork.SalesReturns.AddAsync(salesReturn);
                await _unitOfWork.BoxTransactions.AddAsync(boxTransaction);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
        }
    }
}
