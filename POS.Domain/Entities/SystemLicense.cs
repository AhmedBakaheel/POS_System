using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Domain.Entities
{
    public class SystemLicense
    {
        public int Id { get; set; }
        public string MachineCode { get; set; } 
        public string LicenseKey { get; set; }  
        public DateTime ExpiryDate { get; set; }
        public string LicenseType { get; set; } 
        public bool IsActive { get; set; }
    }
}
