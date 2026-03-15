using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Infrastructure.Repositories
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Sale>> GetSalesByDateAsync(DateTime date)
        {
            return await _context.Sales.Where(s => s.SaleDate.Date == date.Date).ToListAsync();
        }

    }
}
