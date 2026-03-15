using POS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Entities.Product> Products { get; }
        IGenericRepository<Entities.Category> Categories { get; }
        IGenericRepository<Entities.Customer> Customers { get; }
        IGenericRepository<Entities.CustomerTransaction> CustomerTransactions { get; }
        IGenericRepository<BoxTransaction> BoxTransactions { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<Purchase> Purchases { get; }
        IGenericRepository<PurchaseItem> PurchaseItems { get; }
        IGenericRepository<SalesReturn> SalesReturns { get; }
        IGenericRepository<SalesReturnItem> SalesReturnItems { get; }
        IGenericRepository<Shift> Shifts { get; }
        IGenericRepository<SupplierTransaction> SupplierTransactions { get; }
        ISaleRepository Sales { get; }
        ILicenseRepository Licenses { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
