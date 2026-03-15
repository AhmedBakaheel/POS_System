using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.ViewModels
{
    public class PurchaseItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; } 
        public decimal UnitCost { get; set; }
    }
}
