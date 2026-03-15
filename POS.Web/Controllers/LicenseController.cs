using Microsoft.AspNetCore.Mvc;
using POS.Application.Services;

namespace POS.Web.Controllers
{
    public class LicenseController : Controller
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        // شاشة عرض حالة الترخيص
        public async Task<IActionResult> Index()
        {
            var status = await _licenseService.GetCurrentLicenseStatusAsync();
            return View(status); // سنقوم بإنشاء هذه الواجهة الآن
        }

        // عملية التفعيل
        [HttpPost]
        public async Task<IActionResult> Activate(string licenseKey)
        {
            var result = await _licenseService.ActivateLicenseAsync(licenseKey);
            if (result)
            {
                return RedirectToAction("Index", "Home"); // التوجه للرئيسية بعد التفعيل
            }

            ViewBag.Error = "مفتاح الترخيص غير صحيح أو حدث خطأ أثناء التفعيل";
            var status = await _licenseService.GetCurrentLicenseStatusAsync();
            return View("Index", status);
        }
    }
}