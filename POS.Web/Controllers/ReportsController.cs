using Microsoft.AspNetCore.Mvc;
using POS.Application.Services;
using POS.Application.ViewModels;
using POS.Domain.Interfaces;

namespace POS.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        public ReportsController(IUnitOfWork unitOfWork, ISaleService saleService, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _saleService = saleService;
            _productService = productService;
        }

        public async Task<IActionResult> BoxReport()
        {
            var today = DateTime.Today;
            var transactions = await _unitOfWork.BoxTransactions.GetAllAsync();

            var todayTransactions = transactions
                .Where(t => t.TransactionDate.Date == today)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return View(todayTransactions);
        }
        [HttpPost]
        public async Task<IActionResult> AddExpense(decimal amount, string description)
        {
            var result = await _saleService.RecordExpenseAsync(amount, description);

            if (result)
                return Ok(new { message = "تم تسجيل المصروف وتحديث الصندوق" });

            return BadRequest();
        }

        public async Task<IActionResult> BoxReconciliation()
        {
            var closedShifts = await _unitOfWork.Shifts.FindAsync(s => s.IsClosed);

            var report = closedShifts.Select(s => new BoxReportViewModel
            {
                ShiftId = s.Id,
                EmployeeName = s.User?.FullName ?? "موظف غير معروف",
                Date = s.StartTime,
                StartingCash = s.StartingCash,
                ExpectedCash = s.ExpectedCash ?? 0m,
                ActualCash = s.ActualCash ?? 0m
            }).OrderByDescending(r => r.Date).ToList();

            return View(report);
        }
        public async Task<IActionResult> LowStock()
        {
            var products = await _productService.GetLowStockProductsAsync();
            return View(products);
        }
    }
}
