using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class SaleHistoryDTO
    {
        public int SaleId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; } // قيمة الفاتورة
    }
}
