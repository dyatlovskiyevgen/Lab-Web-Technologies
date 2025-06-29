using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using OSS.API.Controllers;
using OSS.UI.Controllers;
using OSS.UI.Services.CategoryService;
using OSS.UI.Services.ProductService;
using OSS30333.API.Data;
using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Tests
{

    public class ProductAPIControllerTests : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private readonly IWebHostEnvironment _environment;

        public ProductAPIControllerTests()
        {
            _environment = Substitute.For<IWebHostEnvironment>();
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Инициализация базы с корректными данными
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();

            var categories = new Category[]
            {
            new Category {Id = 1, Name = "Сухие корма", NormalizedName = "dry-food"},
            new Category {Id = 2, Name = "Консервы", NormalizedName = "canned-food"}
            };
            context.Categories.AddRange(categories);

            var products = new List<Product>
        {
            new Product {
                Id = 1,
                Name = "Royal Canin Sterilised",
                Description="корм",
                CategoryId = 1,
                Category = categories[0],
                Price = 53
            },
            new Product {
                Id = 2,
                Name = "Acana Pacifica CAT",
                Description="корм",
                CategoryId = 1,
                Category = categories[0],
                Price = 92
            },
            new Product {
                Id = 3,
                Name = "Cennamo Prestige",
                Description="корм",
                CategoryId = 1,
                Category = categories[0],
                Price = 36
            },
            new Product {
                Id = 4,
                Name = "Banditos Ягненок",
                Description="корм",
                CategoryId = 2,
                Category = categories[1],
                Price = 2
            },
            new Product {
                Id = 5,
                Name = "Whiskas Курица",
                Description="корм",
                CategoryId = 2,
                Category = categories[1],
                Price = 31
            }
        };
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        public void Dispose() => _connection?.Dispose();

        AppDbContext CreateContext() => new AppDbContext(_contextOptions);
        [Fact]
        public async Task ControllerFiltersCategory()
        {
            // Arrange
            using var context = CreateContext();

            // Проверяем, что тестовые данные загрузились
            var testCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.NormalizedName == "dry-food");
            Assert.NotNull(testCategory); // Убедимся, что категория существует

            var productsInCategory = await context.Products
                .Where(p => p.Category.NormalizedName == "dry-food")
                .CountAsync();
            Assert.True(productsInCategory > 0); // Убедимся, что есть продукты в категории

            var controller = new ProductsAPIController(context, _environment);

            // Act
            var actionResult = await controller.GetProducts("dry-food");

            // Assert - Проверяем структуру ответа
            Assert.NotNull(actionResult);

            var result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result); // Проверяем, что вернулся OkObjectResult

            var responseData = result.Value as ResponseData<ProductListModel<Product>>;
            Assert.NotNull(responseData); // Проверяем структуру ResponseData

            Assert.NotNull(responseData.Data); // Проверяем наличие Data
            Assert.NotNull(responseData.Data.Items); // Проверяем наличие Items

            // Проверяем фильтрацию
            Assert.All(responseData.Data.Items, p =>
                Assert.Equal("dry-food", p.Category?.NormalizedName?.ToLower()));
        }

        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        public async Task ControllerReturnsCorrectPagesCount(int size, int expectedPages)
        {
            // Arrange
            using var context = CreateContext();

            // Проверяем тестовые данные
            var totalProducts = await context.Products.CountAsync();
            Assert.Equal(5, totalProducts); // В тестовых данных должно быть ровно 5 продуктов

            var controller = new ProductsAPIController(context, _environment);

            // Act
            var actionResult = await controller.GetProducts(null, 1, size);

            // Assert - Проверяем структуру ответа
            Assert.NotNull(actionResult);

            var result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result);

            var responseData = result.Value as ResponseData<ProductListModel<Product>>;
            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Data);

            // Проверяем количество страниц
            Assert.Equal(expectedPages, responseData.Data.TotalPages);
        }

        [Fact]
        public async Task ControllerReturnsCorrectPage()
        {
            // Arrange
            using var context = CreateContext();

            // Получаем все продукты с их категориями
            var allProducts = await context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Id)
                .ToListAsync();

            Console.WriteLine("All products ordered by ID:");
            foreach (var p in allProducts)
            {
                Console.WriteLine($"ID: {p.Id}, Name: {p.Name}");
            }

            // Проверяем, что у нас достаточно данных
            Assert.True(allProducts.Count >= 5, "Тест требует минимум 5 продуктов в БД");

            // Ожидаемый первый продукт на второй странице при pageSize=2
            // Страница 1: ID 1, 2
            // Страница 2: ID 3, 4  ← именно эту страницу запрашиваем
            var expectedFirstProductOnPage2 = allProducts[2]; // Третий продукт (индекс 2) должен быть первым на странице 2

            var controller = new ProductsAPIController(context, _environment);

            // Act - запрашиваем вторую страницу (pageNo=2) с размером 2 элемента (pageSize=2)
            var actionResult = await controller.GetProducts(null, 2, 2);

            // Assert
            Assert.NotNull(actionResult);

            // Обрабатываем оба варианта возврата данных
            ResponseData<ProductListModel<Product>> responseData;
            if (actionResult.Result == null)
            {
                responseData = actionResult.Value;
            }
            else
            {
                var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
                responseData = Assert.IsType<ResponseData<ProductListModel<Product>>>(okResult.Value);
            }

            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Data);
            Assert.NotNull(responseData.Data.Items);

            var pageProducts = responseData.Data.Items;

            Console.WriteLine($"Products on page: {string.Join(", ", pageProducts.Select(p => p.Id))}");

            // Проверяем содержимое страницы
            Assert.Equal(2, pageProducts.Count);
            Assert.Equal(expectedFirstProductOnPage2.Id, pageProducts[0].Id);
            Assert.Equal(2, responseData.Data.CurrentPage);
        }



        //[Fact]
        //public async Task ControllerReturnsCorrectPage()
        //{
        //    // Arrange
        //    using var context = CreateContext();

        //    // Проверяем и выводим тестовые данные для отладки
        //    var allProducts = await context.Products
        //        .Include(p => p.Category)
        //        .OrderBy(p => p.Id)
        //        .ToListAsync();

        //    Console.WriteLine("All products in test database:");
        //    foreach (var p in allProducts)
        //    {
        //        Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Category: {p.Category?.NormalizedName}");
        //    }

        //    Assert.True(allProducts.Count >= 5, "В тестовой БД должно быть минимум 5 продуктов");

        //    var expectedFirstProductOnPage2 = allProducts.Skip(3).First();
        //    var controller = new ProductsAPIController(context, _environment);

        //    // Act - получаем вторую страницу (pageNo=2) с размером страницы 2 (pageSize=2)
        //    var actionResult = await controller.GetProducts(null, 2, 2);

        //    // Assert - Проверяем базовую структуру ответа
        //    Assert.NotNull(actionResult);

        //    // Вариант 1: Если метод возвращает ActionResult<...> напрямую
        //    if (actionResult.Result == null)
        //    {
        //        var responseDataDirect = actionResult.Value;
        //        Assert.NotNull(responseDataDirect);
        //        Assert.NotNull(responseDataDirect.Data);
        //        Assert.NotNull(responseDataDirect.Data.Items);

        //        var productsDirect = responseDataDirect.Data.Items;
        //        Assert.Equal(2, productsDirect.Count);
        //        Assert.Equal(expectedFirstProductOnPage2.Id, productsDirect[0].Id);
        //        Assert.Equal(2, responseDataDirect.Data.CurrentPage);
        //        return;
        //    }

        //    // Вариант 2: Если метод возвращает через OkObjectResult
        //    var okResult = actionResult.Result as OkObjectResult;
        //    Assert.NotNull(okResult);

        //    var responseDataOk = okResult.Value as ResponseData<ProductListModel<Product>>;
        //    Assert.NotNull(responseDataOk);
        //    Assert.NotNull(responseDataOk.Data);
        //    Assert.NotNull(responseDataOk.Data.Items);

        //    var productsOk = responseDataOk.Data.Items;
        //    Assert.Equal(2, productsOk.Count);
        //    Assert.Equal(expectedFirstProductOnPage2.Id, productsOk[0].Id);
        //    Assert.Equal(2, responseDataOk.Data.CurrentPage);
        //}
    }




//    public class ProductAPIControllerTests : IDisposable
//    {
//        private readonly DbConnection _connection;
//        private readonly DbContextOptions<AppDbContext> _contextOptions;
//        private readonly IWebHostEnvironment _environment;
//        public ProductAPIControllerTests()
//        {
//            _environment = Substitute.For<IWebHostEnvironment>();

//            // Create and open a connection. This creates the SQLite in-memory database,  which will persist until the connection is closed
//                    // at the end of the test (see Dispose below). 
//                    _connection = new SqliteConnection("Filename=:memory:");
//            _connection.Open();

//            // These options will be used by the context instances in this test suite, including the connection opened above.
//                    _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
//                        .UseSqlite(_connection)
//                        .Options;

//            // Create the schema and seed some data 
//            using var context = new AppDbContext(_contextOptions);

//            context.Database.EnsureCreated();

//            var categories = new Category[]
//{
//    new Category {Name="Сухие корма", NormalizedName="dry-food"},
//    new Category {Name="Консервы", NormalizedName="canned-food"}
//};
//            context.Categories.AddRange(categories);
//            context.SaveChanges();

//            var products = new List<Product>
//            {                
//                new Product {Name="Royal Canin Sterilised",
//                    Description="сухой корм, для пожилых, основной компонент: злаки, образ жизни: домашний, для стерилизованных",
//                    Price =53,
//                    CategoryId = categories[0].Id, // Явно указываем CategoryId
//                    Category = categories[0] },      // И связываем объекты,
//                //new Product { Name="Acana Pacifica CAT",
//                //    Description="сухой корм, для котят, для взрослых, для пожилых, основной компонент: рыба, образ жизни: домашний, беззерновой",
//                //    Price =92,
//                //    Category= categories.FirstOrDefault(c=>c.NormalizedName.Equals("dry-food"))},
//                //new Product { Name="Cennamo Prestige Sterilized Cat",
//                //    Description="Сухой корм для взрослых стерилизованных кошек с сельдью",
//                //    Price =36, Image="Images/3.png",
//                //    Category= categories.FirstOrDefault(c=>c.NormalizedName.Equals("dry-food"))},
//                //new Product { Name=" ",
//                //    Description=" ",
//                //    Price =31,
//                //    Category= categories.FirstOrDefault(c=>c.NormalizedName.Equals("canned-food"))},
//                //new Product { Name="Banditos Кусочки в желе для кошек (Сочный ягненок)",
//                //    Description="Влажный корм для кошек, ягненок",
//                //    Price =2,
//                //    Category= categories.FirstOrDefault(c=>c.NormalizedName.Equals("canned-food"))}
//            };
//            context.AddRange(products);
//            context.SaveChanges();
//        }
//        public void Dispose() => _connection?.Dispose();
//        AppDbContext CreateContext() => new AppDbContext(_contextOptions);

//        // Проверка фильтра по категории 
//        //[Fact]
//        //public async void ControllerFiltersCategory()
//        //{
//        //    // arrange 
//        //    using var context = CreateContext();
//        //    var category = context.Categories.First();

//        //    var controller = new ProductsAPIController(context, _environment);

//        //    // act 
//        //    var response = await controller.GetProducts(category.NormalizedName);
//        //    ResponseData<ProductListModel<Product>> responseData = response.Value;
//        //    var productsList = responseData.Data.Items; // полученный список объектов 

//        //    //assert 
//        //    Assert.True(productsList.All(d => d.CategoryId == category.Id));
//        //}

//        [Fact]
//        public async void ControllerFiltersCategory()
//        {
//            // Arrange
//            using var context = CreateContext();

//            // Явно выбираем категорию по NormalizedName, а не First()
//            var category = await context.Categories
//                .FirstOrDefaultAsync(c => c.NormalizedName == "dry-food");

//            // Убедимся, что категория найдена
//            Assert.NotNull(category);

//            var controller = new ProductsAPIController(context, _environment);

//            // Act
//            var response = await controller.GetProducts(category.NormalizedName);

//            // Проверяем, что ответ не null
//            Assert.NotNull(response);

//            // Проверяем, что Value не null
//            var responseData = response.Value;
//            Assert.NotNull(responseData);

//            // Проверяем, что Data не null
//            Assert.NotNull(responseData.Data);

//            // Проверяем, что Items не null и содержит элементы
//            var productsList = responseData.Data.Items;
//            Assert.NotNull(productsList);
//            Assert.NotEmpty(productsList);

//            // Assert: Все продукты должны принадлежать выбранной категории
//            Assert.All(productsList, p => Assert.Equal(category.Id, p.CategoryId));
//        }



//        // Проверка подсчета количества страниц 
//        // Первый параметр - размер страницы 
//        // Второй параметр - ожидаемое количество страниц (при условии, что всего объектов 5) 
//        [Theory]
//        [InlineData(2, 3)]
//        [InlineData(3, 2)]
//        public async void ControllerReturnsCorrectPagesCount(int size, int qty)
//        {
//            using var context = CreateContext();
//            var controller = new ProductsAPIController(context, _environment);            

//        // act 
//        var response = await controller.GetProducts(null, 1, size);
//            ResponseData<ProductListModel<Product>> responseData = response.Value;
//            var totalPages = responseData.Data.TotalPages; // полученное количество страниц

//        //assert 
//        Assert.Equal(qty, totalPages); // количество страниц совпадает 
//        }

//        [Fact]
//        public async void ControllerReturnsCorrectPage()
//        {
//            using var context = CreateContext();
//            var controller = new ProductsAPIController(context, _environment);
//            // При размере страницы 3 и общем количестве объектов 5 
//            // на 2-й странице должно быть 2 объекта 
//            int itemsInPage = 2;
//            // Первый объект на второй странице 
//            Product firstItem = context.Products.ToArray()[3];

//            // act 
//            // Получить данные 2-й страницы 
//            var response = await controller.GetProducts(null, 2);
//            ResponseData<ProductListModel<Product>> responseData = response.Value;
//            var productsList = responseData.Data.Items; // полученный список объектов 
//            var currentPage = responseData.Data.CurrentPage; // полученный номер текущей  страницы

//            //assert 
//            Assert.Equal(2, currentPage);// номер страницы совпадает 
//            Assert.Equal(2, productsList.Count); // количество объектов на странице равно  2
//            Assert.Equal(firstItem.Id, productsList[0].Id); // 1-й объект в списке  правильный


//        }

    //}
}
