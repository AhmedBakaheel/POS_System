using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using POS.Application.DTOs;
using POS.Application.Services;

namespace POS.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            ViewBag.LowStockCount = products.Count(p => p.StockQuantity <= p.ReorderLevel && p.StockQuantity > 0);
            ViewBag.OutOfStockCount = products.Count(p => p.StockQuantity <= 0);

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View(new ProductDTO { StockQuantity = 0, ReorderLevel = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateProductAsync(productDto);
                    TempData["Success"] = "تم حفظ المنتج بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", "حدث خطأ أثناء الحفظ: " + errorMessage);
                }
            }

            await PopulateCategoriesAsync();
            return View(productDto);
        }

        private async Task PopulateCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productDto = await _productService.GetProductByIdAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }

            await PopulateCategoriesAsync();
            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDTO productDto)
        {
            if (id != productDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(productDto);
                    TempData["Success"] = "تم تحديث بيانات المنتج بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", "خطأ في التحديث: " + errorMessage);
                }
            }

            await PopulateCategoriesAsync();
            return View(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}