using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class SaleDTO
    {
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = "عميل نقدي";
        public int PaymentType { get; set; }
        public int ShiftId { get; set; }
        public List<SaleDetailDTO> Items { get; set; } = new List<SaleDetailDTO>();
        public decimal TotalAmount => Items.Sum(i => i.Total);
        public decimal TaxAmount => 0;
        public decimal GrandTotal => TotalAmount + TaxAmount;
        public decimal TotalDiscount { get; set; } 
        public int? ActivePromotionId { get; set; }
    }
}
