using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class SalesReturnItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SalesReturnId { get; set; }

        [ForeignKey("SalesReturnId")]
        public SalesReturn SalesReturn { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Quantity { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; } 

        [NotMapped]
        public decimal SubTotal => Quantity * UnitPrice; 
    }
}
