using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } // [cite: 66]
        public string Description { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
