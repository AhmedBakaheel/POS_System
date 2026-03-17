using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class SalesDiscount
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        public int? PromotionId { get; set; }
        public decimal DiscountAmount { get; set; } 
    }
}
