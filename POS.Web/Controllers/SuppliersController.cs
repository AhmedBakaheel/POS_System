using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS.Application.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Domain.Interfaces;

namespace POS.Web.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public SuppliersController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var suppliersDto = _mapper.Map<IEnumerable<SupplierDTO>>(suppliers);
            return View(suppliersDto);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierDTO supplierDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var supplier = _mapper.Map<Supplier>(supplierDto);
                    await _unitOfWork.Suppliers.AddAsync(supplier);
                    await _unitOfWork.CompleteAsync();

                    TempData["SuccessMessage"] = "تم إضافة المورد بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "حدث خطأ أثناء الحفظ: " + ex.Message);
                }
            }
            return View(supplierDto);
        }
        public async Task<IActionResult> Statement(int id, DateTime? fromDate, DateTime? toDate)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null) return NotFound();

            var start = fromDate ?? DateTime.MinValue;
            var end = toDate ?? DateTime.Now;

            var allTransactions = await _unitOfWork.SupplierTransactions
                .FindAsync(t => t.SupplierId == id);

            var transactions = allTransactions
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .ToList();

            var openingBalance = allTransactions
                .Where(t => t.Date < start)
                .Sum(t => t.Credit - t.Debit);

            var currentBalance = allTransactions.Sum(t => t.Credit - t.Debit);

            ViewBag.SupplierName = supplier.Name;
            ViewBag.CurrentBalance = currentBalance;
            ViewBag.OpeningBalance = openingBalance;
            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");

            return View(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> PaySupplier(int supplierId, decimal amount, string notes)
        {
            if (amount <= 0)
                return Json(new { success = false, message = "يرجى إدخال مبلغ صحيح أكبر من صفر." });

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(supplierId);
            if (supplier == null)
                return Json(new { success = false, message = "المورد غير موجود." });

            var userId = _userManager.GetUserId(User);
            var activeShift = (await _unitOfWork.Shifts
                .FindAsync(s => s.UserId == userId && !s.IsClosed))
                .FirstOrDefault();

            if (activeShift == null)
                return Json(new { success = false, message = "عذراً، يجب فتح وردية (Shift) أولاً لتتمكن من صرف مبالغ نقدية." });

            try
            {
                var supplierTrans = new SupplierTransaction
                {
                    SupplierId = supplierId,
                    Date = DateTime.Now,
                    Type = SupplierTransactionType.CashPayment,
                    Debit = amount, 
                    Credit = 0,
                    Reference = string.IsNullOrEmpty(notes) ? "سند صرف نقدي" : notes,
                    ShiftId = activeShift.Id
                };
                await _unitOfWork.SupplierTransactions.AddAsync(supplierTrans);

                var boxTrans = new BoxTransaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = amount,
                    Type = TransactionType.ReturnSale, 
                    Description = $"صرف نقدية للمورد: {supplier.Name} - {notes}",
                    ShiftId = activeShift.Id,
                    UserId = userId
                };
                await _unitOfWork.BoxTransactions.AddAsync(boxTrans);
                supplier.Balance -= amount;
                _unitOfWork.Suppliers.Update(supplier);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true, message = $"تم صرف مبلغ {amount:N2} للمورد بنجاح وتحديث الرصيد." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء معالجة العملية: " + ex.Message });
            }
        }
    }

}
