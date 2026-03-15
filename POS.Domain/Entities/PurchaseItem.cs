using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; } 
        public int PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }
    }
}
