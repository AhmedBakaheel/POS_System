using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class SalesReturnDTO
    {
        public int SaleId { get; set; }  
        public int ShiftId { get; set; }  
        public string Reason { get; set; }    
        public decimal TotalAmount { get; set; }  
        public List<SalesReturnItemDTO> Items { get; set; } = new List<SalesReturnItemDTO>();
    }
}
