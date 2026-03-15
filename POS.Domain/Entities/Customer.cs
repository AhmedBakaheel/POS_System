using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Phone { get; set; }
        public decimal Balance { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Sale> Sales { get; set; }
        public virtual ICollection<CustomerTransaction> Transactions { get; set; }
    }
}
