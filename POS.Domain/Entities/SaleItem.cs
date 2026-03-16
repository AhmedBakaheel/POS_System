using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public int SaleId { get; set; }
        public int? HeldSaleId { get; set; }
        public virtual Sale Sale { get; set; }
        public virtual HeldSale HeldSale { get; set; }
    }
}
