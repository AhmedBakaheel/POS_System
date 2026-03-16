using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class HeldSaleDTO
    {
        public string? Note { get; set; }
        public List<SaleDetailDTO> Items { get; set; } = new List<SaleDetailDTO>();
    }
}
