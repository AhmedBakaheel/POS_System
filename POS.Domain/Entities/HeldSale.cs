using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class HeldSale
    {
        public int Id { get; set; }
        public DateTime HoldDate { get; set; } = DateTime.Now;
        public string? Note { get; set; } 
        public string UserId { get; set; }
        public string ContentJson { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
