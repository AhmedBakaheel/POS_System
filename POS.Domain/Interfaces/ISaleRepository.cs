using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Interfaces
{
    public interface ISaleRepository : IGenericRepository<Entities.Sale>
    {
        Task<IEnumerable<Entities.Sale>> GetSalesByDateAsync(DateTime date);
    }
   
}
