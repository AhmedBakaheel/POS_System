using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class ShiftResultDTO
    {
        public int ShiftId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public decimal StartingCash { get; set; }

        public decimal TotalTransactions { get; set; }

        public decimal Expected { get; set; }

        public decimal Actual { get; set; }

        public decimal Difference { get; set; }

        public string Status
        {
            get
            {
                if (Difference == 0) return "متطابق";
                return Difference > 0 ? $"زيادة بمقدار {Difference:N2}" : $"عجز بمقدار {Math.Abs(Difference):N2}";
            }
        }
    }
}
