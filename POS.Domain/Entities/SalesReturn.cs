using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class SalesReturn
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public Sale Sale { get; set; } 
        public DateTime ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Reason { get; set; }
        public int ShiftId { get; set; }
        public virtual ICollection<SalesReturnItem> ReturnItems { get; set; } = new List<SalesReturnItem>();
    }
}
