using POS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class CustomerTransaction
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [StringLength(250)]
        public string Description { get; set; }
        public string Reference { get; set; }
        public decimal Debit { get; set; }  
        public decimal Credit { get; set; } 
        public CustomerTransactionType Type { get; set; }
    }
}
