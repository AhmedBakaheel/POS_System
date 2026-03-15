using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.ViewModels
{
    public class CustomerStatementRow
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public decimal Debit { get; set; }  
        public decimal Credit { get; set; } 
        public decimal Balance { get; set; } 
    }
}
