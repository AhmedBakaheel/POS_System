using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class CustomerStatementDTO
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal CurrentBalance { get; set; } // الرصيد الحالي (المديونية)
        public List<SaleHistoryDTO> SalesHistory { get; set; } = new List<SaleHistoryDTO>();
    }
}
