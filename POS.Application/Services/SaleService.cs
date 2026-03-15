using AutoMapper;
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
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SaleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> ProcessSaleAsync(SaleDTO saleDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var sale = new Sale
                {
                    CustomerId = saleDto.CustomerId,
                    PaymentType = (PaymentType)saleDto.PaymentType,
                    GrandTotal = saleDto.GrandTotal,
                    ShiftId = saleDto.ShiftId,
                    SaleDate = DateTime.Now,
                    SaleItems = new List<SaleItem>()
                };

                foreach (var item in saleDto.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null || product.StockQuantity < item.Quantity)
                    {
                        await _unitOfWork.RollbackAsync();
                        return false;
                    }

                    // خصم المخزن
                    product.StockQuantity -= item.Quantity;
                    _unitOfWork.Products.Update(product);

                    sale.SaleItems.Add(new SaleItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                // إضافة سجل حركة الصندوق إذا كان الدفع نقداً
                if (sale.PaymentType == PaymentType.Cash)
                {
                    await _unitOfWork.BoxTransactions.AddAsync(new BoxTransaction
                    {
                        TransactionDate = DateTime.Now,
                        Type = TransactionType.Sale,
                        Amount = sale.GrandTotal,
                        Description = $"مبيعات فاتورة رقم #{sale.Id}",
                        ShiftId = sale.ShiftId
                    });
                }

                await _unitOfWork.Sales.AddAsync(sale);
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

        public async Task<CustomerStatementDTO> GetCustomerStatementAsync(int customerId)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null) return null;

            var allSales = await _unitOfWork.Sales.GetAllAsync();
            var customerSales = allSales.Where(s => s.CustomerId == customerId && s.PaymentType == PaymentType.Credit).ToList();

            return new CustomerStatementDTO
            {
                CustomerId = customer.Id,
                CustomerName = customer.Name,
                CurrentBalance = customer.Balance,
                SalesHistory = customerSales.Select(s => new SaleHistoryDTO
                {
                    SaleId = s.Id,
                    Date = s.SaleDate,
                    Amount = s.GrandTotal
                }).OrderByDescending(s => s.Date).ToList()
            };
        }

        public async Task<bool> RecordCustomerPaymentAsync(int customerId, decimal amount)
        {
            if (amount <= 0) return false;

            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer == null) return false;

            customer.Balance -= amount;
            _unitOfWork.Customers.Update(customer);

            var paymentEntry = new BoxTransaction
            {
                TransactionDate = DateTime.Now,
                Type = TransactionType.Payment, // Payment من العميل يزيد الصندوق
                Amount = amount,
                Description = $"سند قبض - العميل: {customer.Name}"
            };
            await _unitOfWork.BoxTransactions.AddAsync(paymentEntry);

            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> RecordExpenseAsync(decimal amount, string description)
        {
            if (amount <= 0) return false;

            var expenseEntry = new BoxTransaction
            {
                TransactionDate = DateTime.Now,
                Type = TransactionType.Expense,
                Amount = amount, // يتم تخزينه كموجب والمنطق المحاسبي يطرحه عند الجرد
                Description = description
            };

            await _unitOfWork.BoxTransactions.AddAsync(expenseEntry);
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
