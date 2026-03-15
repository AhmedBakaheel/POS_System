using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; } 
        public decimal Cost { get; set; } 
        public decimal StockQuantity { get; set; } 
        public int ReorderLevel { get; set; } = 5;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
