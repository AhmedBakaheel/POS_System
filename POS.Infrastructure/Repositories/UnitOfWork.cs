using Microsoft.EntityFrameworkCore.Storage;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;  
        private IDbContextTransaction _transaction;
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Customer> Customers { get; private set; }
        public IGenericRepository<CustomerTransaction> CustomerTransactions { get; private set; }
        public IGenericRepository<BoxTransaction> BoxTransactions { get; private set; }
        public IGenericRepository<Supplier> Suppliers { get; private set; } 
        public IGenericRepository<Purchase> Purchases { get; private set; }
        public IGenericRepository<PurchaseItem> PurchaseItems { get; private set; }
        public IGenericRepository<Shift> Shifts { get; private set; }
        public IGenericRepository<SalesReturn> SalesReturns { get; private set; }
        public IGenericRepository<SalesReturnItem> SalesReturnItems { get; private set; }
        public IGenericRepository<SupplierTransaction> SupplierTransactions { get; private set; }
        public ISaleRepository Sales { get; private set; }
        public ILicenseRepository Licenses { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Products = new GenericRepository<Product>(_context);
            Categories = new GenericRepository<Category>(_context);
            Customers = new GenericRepository<Customer>(_context);
            CustomerTransactions = new GenericRepository<CustomerTransaction>(_context);
            Suppliers = new GenericRepository<Supplier>(_context); 
            Purchases = new GenericRepository<Purchase>(_context);
            PurchaseItems = new GenericRepository<PurchaseItem>(_context);
            BoxTransactions = new GenericRepository<BoxTransaction>(_context);
            Shifts = new GenericRepository<Shift>(_context);
            Sales = new SaleRepository(_context);
            Licenses = new LicenseRepository(_context);
            SalesReturns = new GenericRepository<SalesReturn>(_context);
            SalesReturnItems = new GenericRepository<SalesReturnItem>(_context);
            SupplierTransactions = new GenericRepository<SupplierTransaction>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync(); 
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync(); 
                _transaction = null;
            }
        }

        public void Dispose() => _context.Dispose();
    }
}
