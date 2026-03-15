using POS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class SupplierTransaction
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public SupplierTransactionType Type { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public string? Reference { get; set; } 
        public string? Notes { get; set; }
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public int? ShiftId { get; set; }
    }
}
