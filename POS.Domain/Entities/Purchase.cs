using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now; 
        public decimal TotalAmount { get; set; }
        
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; }
        
    }
}
