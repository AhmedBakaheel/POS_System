using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Services;
using POS.Application.ViewModels;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Domain.Interfaces;

namespace POS.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public CustomersController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

       
        public async Task<IActionResult> Index()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return View(customers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = new Customer
            {
                Name = model.Name,
                Phone = model.Phone,
                Balance = model.InitialBalance
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.CompleteAsync();


            if (model.InitialBalance != 0)
            {
                await _unitOfWork.CustomerTransactions.AddAsync(new CustomerTransaction
                {
                    CustomerId = customer.Id,
                    Date = DateTime.Now,
                    Description = "رصيد افتتاحي",
                    Debit = model.InitialBalance > 0 ? model.InitialBalance : 0,
                    Credit = model.InitialBalance < 0 ? Math.Abs(model.InitialBalance) : 0,
                    Type = CustomerTransactionType.OpeningBalance
                });
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Statement(int id, DateTime? fromDate, DateTime? toDate)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null) return NotFound();

            var start = fromDate ?? DateTime.MinValue;
            var end = toDate ?? DateTime.Now;

            var allTransactions = await _unitOfWork.CustomerTransactions.FindAsync(t => t.CustomerId == id);

            var openingBalance = allTransactions
                .Where(t => t.Date < start)
                .Sum(t => t.Debit - t.Credit);

            decimal runningBalance = openingBalance;

            var statementRows = allTransactions
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .Select(t => new CustomerStatementRow
                {
                    Date = t.Date,
                    Description = t.Description,
                    Reference = t.Reference,
                    Debit = t.Debit,
                    Credit = t.Credit,
                    Balance = (runningBalance += (t.Debit - t.Credit))
                })
                .ToList();

            ViewBag.CustomerName = customer.Name;
            ViewBag.OpeningBalance = openingBalance;
            ViewBag.CurrentBalance = customer.Balance; 
            ViewBag.FromDate = start.ToString("yyyy-MM-dd");
            ViewBag.ToDate = end.ToString("yyyy-MM-dd");

            return View(statementRows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(CustomerPaymentViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("بيانات الدفعة غير صحيحة");

            var customer = await _unitOfWork.Customers.GetByIdAsync(model.CustomerId);
            if (customer == null) return NotFound();

            
            customer.Balance -= model.Amount;
            _unitOfWork.Customers.Update(customer);

            
            await _unitOfWork.CustomerTransactions.AddAsync(new CustomerTransaction
            {
                CustomerId = customer.Id,
                Date = model.Date,
                Description = "سند قبض: " + model.Note,
                Debit = 0,
                Credit = model.Amount,
                Type = CustomerTransactionType.Payment
            });

            await _unitOfWork.BoxTransactions.AddAsync(new BoxTransaction
            {
                TransactionDate = model.Date,
                Amount = model.Amount,
                Type = TransactionType.Income,
                Description = $"تحصيل من العميل: {customer.Name}",
                UserId = _userManager.GetUserId(User)
            });

            await _unitOfWork.CompleteAsync();
            return Ok(new { message = "تم تسجيل الدفعة وتحديث الرصيد بنجاح" });
        }
    }
}
