using POS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public interface ISaleService
    {
        Task<bool> ProcessSaleAsync(SaleDTO saleDto);
        Task<CustomerStatementDTO> GetCustomerStatementAsync(int customerId);
        Task<bool> RecordCustomerPaymentAsync(int customerId, decimal amount);
        Task<bool> RecordExpenseAsync(decimal amount, string description);
    }
}
