using POS.Domain.Entities;
using POS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SupplierService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
            => await _unitOfWork.Suppliers.GetAllAsync();

        public async Task<Supplier> GetSupplierByIdAsync(int id)
            => await _unitOfWork.Suppliers.GetByIdAsync(id);

        public async Task CreateSupplierAsync(Supplier supplier)
        {
            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();
        }
    }
}
