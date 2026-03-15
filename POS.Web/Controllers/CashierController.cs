using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Domain.Entities;
using POS.Domain.Interfaces;
using POS.Domain.Enums;

[Authorize]
public class CashierController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public CashierController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult OpenShift()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> OpenShift(decimal startingCash)
    {
        var userId = _userManager.GetUserId(User);

        var activeShift = (await _unitOfWork.Shifts
            .FindAsync(s => s.UserId == userId && !s.IsClosed))
            .FirstOrDefault();

        if (activeShift != null)
        {
            return RedirectToAction("Index", "Sales"); 
        }

        var shift = new Shift
        {
            UserId = userId,
            StartTime = DateTime.Now,
            StartingCash = startingCash,
            IsClosed = false
        };

        await _unitOfWork.Shifts.AddAsync(shift);
        await _unitOfWork.CompleteAsync();

        return RedirectToAction("Index", "Sales");
    }
    [HttpGet]
    public async Task<IActionResult> CloseShift()
    {
        var userId = _userManager.GetUserId(User);
        var activeShift = (await _unitOfWork.Shifts
            .FindAsync(s => s.UserId == userId && !s.IsClosed))
            .FirstOrDefault();

        if (activeShift == null) return RedirectToAction("Index", "Home");

        var transactions = await _unitOfWork.BoxTransactions.FindAsync(t => t.ShiftId == activeShift.Id);

        var creditSales = (await _unitOfWork.Sales
            .FindAsync(s => s.ShiftId == activeShift.Id && s.PaymentType == PaymentType.Credit))
            .Sum(s => s.GrandTotal);

        var totalSales = transactions.Where(t => t.Type == TransactionType.Sale).Sum(t => t.Amount);
        var totalReturns = transactions.Where(t => t.Type == TransactionType.ReturnSale).Sum(t => t.Amount);

        ViewBag.InternalExpected = activeShift.StartingCash + (totalSales - totalReturns);
        ViewBag.CreditSales = creditSales;

        return View(activeShift);
    }

    [HttpPost]
    public async Task<IActionResult> CloseShift(int shiftId, decimal actualCash)
    {
        var shift = await _unitOfWork.Shifts.GetByIdAsync(shiftId);
        if (shift == null) return NotFound();

        var transactions = await _unitOfWork.BoxTransactions.FindAsync(t => t.ShiftId == shiftId);

        var totalSales = transactions.Where(t => t.Type == TransactionType.Sale).Sum(t => t.Amount);
        var totalReturns = transactions.Where(t => t.Type == TransactionType.ReturnSale).Sum(t => t.Amount);
        var netTransactions = totalSales - totalReturns;

        shift.EndTime = DateTime.Now;
        shift.ActualCash = actualCash;
        shift.ExpectedCash = shift.StartingCash + netTransactions;
        shift.IsClosed = true;

        decimal balanceDifference = actualCash - (shift.ExpectedCash ?? 0);

        _unitOfWork.Shifts.Update(shift);
        await _unitOfWork.CompleteAsync();

        if (balanceDifference < 0)
        {
            TempData["ShiftStatus"] = $"تم الإغلاق. يوجد عجز مالي كبير: {Math.Abs(balanceDifference):N2}";
            TempData["StatusType"] = "error";
        }
        else if (balanceDifference > 0)
        {
            TempData["ShiftStatus"] = $"تم الإغلاق. توجد زيادة: {balanceDifference:N2}";
            TempData["StatusType"] = "warning";
        }
        else
        {
            TempData["ShiftStatus"] = "تم إغلاق الوردية بنجاح والحساب متطابق.";
            TempData["StatusType"] = "success";
        }

        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}