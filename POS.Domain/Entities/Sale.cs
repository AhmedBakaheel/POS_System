using POS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public PaymentType PaymentType { get; set; }
        public int? CustomerId { get; set; }
        public int ShiftId { get; set; }
        public string? UserId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual AppUser User { get; set; } 
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
