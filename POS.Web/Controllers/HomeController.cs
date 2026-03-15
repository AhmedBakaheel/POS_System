using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Services;
using POS.Application.ViewModels;
using POS.Domain.Enums;
using POS.Domain.Interfaces;
using POS.Web.Models;

namespace POS.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductService _productService;

    public HomeController(IUnitOfWork unitOfWork, IProductService productService)
    {
        _unitOfWork = unitOfWork;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var sales = await _unitOfWork.Sales.GetAllAsync();
        var todaySalesTotal = sales.Where(s => s.SaleDate.Date == today).Sum(s => s.GrandTotal);

        var purchases = await _unitOfWork.Purchases.GetAllAsync();
        var todayPurchasesCount = purchases.Count(p => p.PurchaseDate.Date == today);

        var lowStockProducts = await _productService.GetLowStockProductsAsync();
        var lowStockCount = lowStockProducts.Count();

        var boxTransactions = await _unitOfWork.BoxTransactions.GetAllAsync();

        var currentBalance = boxTransactions.Sum(t =>
            t.Type == TransactionType.Sale ? t.Amount : -t.Amount
        );

        var viewModel = new DashboardViewModel
        {
            TodaySales = todaySalesTotal,
            TodayPurchasesCount = todayPurchasesCount,
            LowStockCount = lowStockCount,
            BoxBalance = currentBalance
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
