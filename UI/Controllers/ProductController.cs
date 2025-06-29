using Microsoft.AspNetCore.Mvc;
using OSS.UI.Services.CategoryService;
using OSS.UI.Services.ProductService;
using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;

namespace OSS.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [Route("Catalog")]
        [Route("Catalog/{category}")]
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            try
            {
                _logger.LogInformation("Загрузка каталога. Категория: {Category}, Страница: {PageNo}", category, pageNo);

                // Получаем категории
                var categoriesResponse = await _categoryService.GetCategoryListAsync();
                if (!categoriesResponse.Success)
                {
                    _logger.LogWarning("Ошибка при загрузке категорий: {Error}", categoriesResponse.ErrorMessage);
                    ViewData["Error"] = "Ошибка при загрузке категорий";
                    return View(new ProductListModel<Product>());
                }

                ViewData["categories"] = categoriesResponse.Data ?? new List<Category>();
                ViewData["currentCategory"] = GetCurrentCategoryName(category, categoriesResponse.Data);

                // Получаем продукты
                var productResponse = await _productService.GetProductListAsync(category, pageNo);
                if (!productResponse.Success)
                {
                    _logger.LogWarning("Ошибка при загрузке товаров: {Error}", productResponse.ErrorMessage);
                    ViewData["Error"] = productResponse.ErrorMessage;
                    return View(new ProductListModel<Product>());
                }

                _logger.LogInformation("Успешно загружено {Count} товаров", productResponse.Data?.Items?.Count ?? 0);
                return View(productResponse.Data ?? new ProductListModel<Product>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при загрузке каталога");
                ViewData["Error"] = "Произошла непредвиденная ошибка";
                return View(new ProductListModel<Product>());
            }
        }

        private string GetCurrentCategoryName(string? category, List<Category>? categories)
        {
            if (category == null) return "Все";

            var categoryName = categories?
                .FirstOrDefault(c => c.NormalizedName == category)?
                .Name;

            return categoryName ?? "Неизвестная категория";
        }
    }
}
