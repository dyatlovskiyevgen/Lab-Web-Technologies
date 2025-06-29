using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OSS.UI.Controllers;
using OSS.UI.Services.CategoryService;
using OSS.UI.Services.ProductService;
using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSS.Tests
{
    public class ProductControllerTests
    {
        //IProductService _productService;
        //ICategoryService _categoryService;
        //ILogger<ProductController> _logger;

        IProductService _productService = null!;
        ICategoryService _categoryService = null!;
        ILogger<ProductController> _logger = null!;


        public ProductControllerTests()
        {
            SetupData();
        }
        // Список категорий сохраняется во ViewData 
        [Fact]
        public async void IndexPutsCategoriesToViewData()
        {
            //arrange 
            var controller = new ProductController(_productService, _categoryService, _logger);

            //act 
            var response = await controller.Index(null);

            //assert 
            var view = Assert.IsType<ViewResult>(response);
            var categories = Assert.IsType<List<Category>>(view.ViewData["categories"]);
            Assert.Equal(4, categories.Count);
            Assert.Equal("Все", view.ViewData["currentCategory"]);

        }
        // Имя текущей категории сохраняется во ViewData 
        [Fact]
        public async void IndexSetsCorrectCurrentCategory()
        {
            //arrange 
            var categories = await _categoryService.GetCategoryListAsync();
            var currentCategory = categories.Data[0];
            var controller = new ProductController(_productService, _categoryService, _logger);

            //act 
            var response = await controller.Index(currentCategory.NormalizedName);

            //assert 
            var view = Assert.IsType<ViewResult>(response);

            Assert.Equal(currentCategory.Name, view.ViewData["currentCategory"]);
        }
        // В случае ошибки возвращается NotFoundObjectResult 
        [Fact]
        public async void IndexReturnsNotFound()
        {
            //arrange         
            string errorMessage = "Test error";
            var categoriesResponse = new ResponseData<List<Category>>();
            categoriesResponse.Success = false;
            categoriesResponse.ErrorMessage = errorMessage;

            _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));
            var controller = new ProductController(_productService, _categoryService, _logger);

            //act 
            var response = await controller.Index(null);

            //assert 
            //var result = Assert.IsType<NotFoundObjectResult>(response);
            //Assert.Equal(errorMessage, result.Value.ToString());
            //

            var result = Assert.IsType<ViewResult>(response);
            Assert.Equal("Ошибка при загрузке категорий", result.ViewData["Error"]);


        }
        // Настройка имитации ICategoryService и IProductService 
        void SetupData()
        {
            _categoryService = Substitute.For<ICategoryService>();
            _logger = Substitute.For<ILogger<ProductController>>();

            var categoriesResponse = new ResponseData<List<Category>>();
            categoriesResponse.Data = new List<Category>
        {
            new Category {Id=1, Name="Сухие корма",  NormalizedName="dry-food"},
            new Category {Id=2, Name="Консервы",NormalizedName="canned-food"},
            new Category {Id=3, Name="Домики и лежанки",NormalizedName="houses-and-beds"},
            new Category {Id=4, Name="Игрушки",NormalizedName="toys"}
        };

        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse)) ;

        _productService = Substitute.For<IProductService>();

        var products = new List<Product>
        {
            new Product {Id = 1 },
            new Product { Id = 2 },
            new Product { Id = 3 },
            new Product { Id = 4 },
            new Product { Id = 5 }
        };

        var productResponse = new ResponseData<ProductListModel<Product>>();
        productResponse.Data = new ProductListModel<Product> { Items = products };
        _productService.GetProductListAsync(Arg.Any<string?>(), Arg.Any<int>())
            .Returns(productResponse);
    }
    }
    }
