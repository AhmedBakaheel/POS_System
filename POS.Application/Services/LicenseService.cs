using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using POS.Application.DTOs;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Application.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LicenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string GenerateMachineCode()
        {
            // كود جلب معرف المعالج (CPU ID)
            string cpuId = "OFFLINE-DEV-MODE";
            try
            {
                using (ManagementClass mc = new ManagementClass("win32_processor"))
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        cpuId = mo.Properties["ProcessorId"].Value.ToString();
                        break;
                    }
                }
            }
            catch { /* في حال فشل الوصول للهاردوير */ }

            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cpuId)).Substring(0, 15).ToUpper();
        }

        // تنفيذ دالة التفعيل المطلوبة
        public async Task<bool> ActivateLicenseAsync(string key)
        {
            var license = (await _unitOfWork.Licenses.GetAllAsync()).FirstOrDefault();

            if (license == null)
            {
                license = new SystemLicense { MachineCode = GenerateMachineCode() };
                await _unitOfWork.Licenses.AddAsync(license);
            }

            license.LicenseKey = key;
            license.IsActive = true;

            // منطق تجاري: إذا بدأ المفتاح بـ "FREE" يعطيه شهر، وإذا بدأ بـ "FULL" يعطيه مدى الحياة
            if (key.StartsWith("FREE"))
            {
                license.ExpiryDate = DateTime.Now.AddMonths(1);
                license.LicenseType = "Trial";
            }
            else
            {
                license.ExpiryDate = DateTime.Now.AddYears(100); // مدى الحياة
                license.LicenseType = "Lifetime";
            }

            return await _unitOfWork.CompleteAsync() > 0;
        }

        // تنفيذ دالة جلب الحالة المطلوبة
        public async Task<LicenseStatusDTO> GetCurrentLicenseStatusAsync()
        {
            var license = (await _unitOfWork.Licenses.GetAllAsync()).FirstOrDefault();

            if (license == null) return new LicenseStatusDTO { IsActive = false, MachineCode = GenerateMachineCode() };

            return new LicenseStatusDTO
            {
                IsActive = license.IsActive && license.ExpiryDate > DateTime.Now,
                MachineCode = license.MachineCode,
                ExpiryDate = license.ExpiryDate,
                LicenseType = license.LicenseType,
                DaysRemaining = (license.ExpiryDate - DateTime.Now).Days
            };
        }

        public async Task<bool> IsFeatureAccessAllowedAsync()
        {
            var status = await GetCurrentLicenseStatusAsync();
            return status.IsActive;
        }
    }
}
