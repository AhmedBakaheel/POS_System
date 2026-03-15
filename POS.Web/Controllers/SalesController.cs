using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Domain.Entities;
using POS.Application.DTOs;
using POS.Application.Services;
using POS.Domain.Interfaces;
using POS.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

public class SalesController : Controller
{
    private readonly IProductService _productService;
    private readonly ISaleService _saleService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISalesReturnService _salesReturnService;
    public SalesController(
        IProductService productService,
        ISaleService saleService,
        UserManager<AppUser> userManager,
        IUnitOfWork unitOfWork)
    {
        _productService = productService;
        _saleService = saleService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var activeShift = (await _unitOfWork.Shifts
            .FindAsync(s => s.UserId == userId && !s.IsClosed))
            .FirstOrDefault();

        if (activeShift == null)
        {
            return RedirectToAction("OpenShift", "Cashier");
        }

        var customers = await _unitOfWork.Customers.GetAllAsync(); 
        ViewBag.Customers = new SelectList(customers, "Id", "Name");

        ViewBag.ShiftId = activeShift.Id;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SearchProducts(string term)
    {
        if (string.IsNullOrEmpty(term)) return Ok(new List<object>());

        var products = await _productService.GetAllProductsAsync();
        var results = products
            .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                     || p.Barcode.Contains(term))
            .Select(p => new {
                id = p.Id,
                name = p.Name,
                price = p.Price,
                barcode = p.Barcode,
                stock = p.StockQuantity
            }).Take(10);

        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetProductByBarcode(string barcode)
    {
        var product = await _productService.GetProductByBarcodeAsync(barcode);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> ReturnSale()
    {
        return View(); 
    }

    [HttpGet]
    public async Task<IActionResult> GetSaleDetails(int saleId)
    {
        var sale = await _unitOfWork.Sales.FindAsync(s => s.Id == saleId);
        var saleData = sale.FirstOrDefault();

        if (saleData == null) return NotFound("الفاتورة غير موجودة");

        var items = saleData.SaleItems.Select(i => new {
            productId = i.ProductId,
            productName = i.Product?.Name,
            quantity = i.Quantity,
            unitPrice = i.UnitPrice,
            total = i.Quantity * i.UnitPrice
        });

        return Ok(new { id = saleData.Id, items = items, grandTotal = saleData.GrandTotal });
    }

    [HttpPost]
    public async Task<IActionResult> ProcessReturn([FromBody] SalesReturnDTO returnDto)
    {
        var result = await _salesReturnService.ProcessReturnAsync(returnDto);
        return result ? Ok() : BadRequest("فشل تنفيذ المرتجع");
    }
    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] SaleDTO saleDto)
    {
        if (saleDto == null || !saleDto.Items.Any())
            return BadRequest("الفاتورة فارغة");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                GrandTotal = saleDto.GrandTotal,
                TotalAmount = saleDto.GrandTotal, 
                TaxAmount = 0,
                PaymentType = (PaymentType)saleDto.PaymentType,
                CustomerId = saleDto.CustomerId,
                ShiftId = saleDto.ShiftId,
                UserId = _userManager.GetUserId(User)
            };

            foreach (var item in saleDto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                if (product == null || product.StockQuantity < item.Quantity)
                    throw new Exception($"المنتج {product?.Name} رصيده غير كافٍ");

                product.StockQuantity -= item.Quantity;

                sale.SaleItems.Add(new SaleItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            await _unitOfWork.Sales.AddAsync(sale);

            if (saleDto.PaymentType == 2 && saleDto.CustomerId.HasValue)
            {
                await _unitOfWork.CustomerTransactions.AddAsync(new CustomerTransaction
                {
                    CustomerId = saleDto.CustomerId.Value,
                    Date = DateTime.Now,
                    Description = $"فاتورة مبيعات رقم {sale.Id}",
                    Reference = sale.Id.ToString(),
                    Debit = saleDto.GrandTotal,
                    Credit = 0,
                    Type = CustomerTransactionType.SaleInvoice
                });
            }

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitAsync();

            return Ok(new { saleId = sale.Id });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return BadRequest(ex.Message);
        }
    }
}