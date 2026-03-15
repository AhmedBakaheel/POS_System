using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.DTOs
{
    public class LicenseStatusDTO
    {
        public bool IsActive { get; set; }
        public string MachineCode { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string LicenseType { get; set; } // تجريبي، سنوي، دائم 
        public int DaysRemaining { get; set; }
    }
}
