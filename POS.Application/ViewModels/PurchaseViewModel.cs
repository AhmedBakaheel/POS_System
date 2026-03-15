using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.ViewModels
{
    public class PurchaseViewModel
    {
        public int SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        public List<PurchaseItemViewModel> Items { get; set; } = new List<PurchaseItemViewModel>();
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
