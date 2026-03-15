using POS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Services
{
    public interface ILicenseService
    {
        string GenerateMachineCode(); // توليد كود الجهاز الفريد [cite: 82]
        Task<bool> ActivateLicenseAsync(string key); // تفعيل النظام بالمفتاح [cite: 84]
        Task<LicenseStatusDTO> GetCurrentLicenseStatusAsync(); // فحص الحالة الحالية
        Task<bool> IsFeatureAccessAllowedAsync(); // هل مسموح له بإصدار فاتورة؟ [cite: 87]
    }
}
