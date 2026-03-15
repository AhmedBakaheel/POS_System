using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using POS.Application.DTOs;
using POS.Application.Services;
using POS.Application.ViewModels;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Domain.Interfaces;

namespace POS.Web.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public PurchasesController(
            IPurchaseService purchaseService,
            ISupplierService supplierService,
            IProductService productService,
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager)
        {
            _purchaseService = purchaseService;
            _supplierService = supplierService;
            _productService = productService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return View(purchases);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateProducts();
            await PopulateSuppliers();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            // 1. التحقق من وجود أصناف
            if (model.Items == null || model.Items.Count == 0)
            {
                await PopulateProducts();
                await PopulateSuppliers();
                return View(model);
            }

            try
            {
                // 2. إنشاء رأس الفاتورة
                var purchase = new Purchase
                {
                    SupplierId = model.SupplierId,
                    PurchaseDate = model.PurchaseDate,
                    TotalAmount = model.GrandTotal
                };

                await _unitOfWork.Purchases.AddAsync(purchase);
                await _unitOfWork.CompleteAsync(); // نحفظ هنا لنحصل على Id الفاتورة

                // 3. إضافة حركات الأصناف وتحديث المخزن
                foreach (var item in model.Items)
                {
                    var purchaseItem = new PurchaseItem
                    {
                        PurchaseId = purchase.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitCost = item.UnitCost
                    };
                    await _unitOfWork.PurchaseItems.AddAsync(purchaseItem);

                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += item.Quantity;
                        product.Cost = item.UnitCost;
                        _unitOfWork.Products.Update(product);
                    }
                }

                // 4. تحديث كشف حساب المورد (الجزء المفقود الذي سبب المشكلة)
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(model.SupplierId);
                if (supplier != null)
                {
                    // إضافة حركة "دائن" (Credit) بقيمة الفاتورة كاملة في كشف الحساب
                    var transaction = new SupplierTransaction
                    {
                        SupplierId = supplier.Id,
                        Date = model.PurchaseDate,
                        Type = SupplierTransactionType.PurchaseInvoice, // تأكد من وجود هذا النوع في الـ Enum
                        Credit = model.GrandTotal, // الفاتورة تزيد مديونية المورد
                        Debit = 0,
                        Reference = "فاتورة شراء رقم " + purchase.Id
                    };
                    await _unitOfWork.SupplierTransactions.AddAsync(transaction);

                    // إذا دفع المستخدم مبلغاً نقدياً عند الشراء
                    if (model.PaidAmount > 0)
                    {
                        var paymentTransaction = new SupplierTransaction
                        {
                            SupplierId = supplier.Id,
                            Date = DateTime.Now,
                            Type = SupplierTransactionType.CashPayment,
                            Credit = 0,
                            Debit = model.PaidAmount, // الدفع ينقص مديونية المورد
                            Reference = "دفعة نقدية للفاتورة رقم " + purchase.Id
                        };
                        await _unitOfWork.SupplierTransactions.AddAsync(paymentTransaction);
                    }
                }

                // 5. حفظ كل التغييرات المالية والمخزنية دفعة واحدة
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء الحفظ: " + ex.Message);
                await PopulateProducts();
                await PopulateSuppliers();
                return View(model);
            }
        }
        private async Task PopulateProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            ViewBag.Products = products;
        }
        private async Task PopulateSuppliers()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.Suppliers = suppliers;
        }
    }
}