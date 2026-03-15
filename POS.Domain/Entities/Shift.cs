using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class Shift
    {
        public int Id { get; set; }

        // ربط الوردية بالمستخدم (الموظف)
        public string UserId { get; set; }
        public AppUser User { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public decimal StartingCash { get; set; }   // المبلغ الافتتاحي (الفكة)
        public decimal? ExpectedCash { get; set; } // ما يتوقعه النظام (حساب آلي)
        public decimal? ActualCash { get; set; }   // ما وجده الموظف (جرد يدوي)

        public bool IsClosed { get; set; } = false;

        public ICollection<BoxTransaction> Transactions { get; set; }
    }
}
