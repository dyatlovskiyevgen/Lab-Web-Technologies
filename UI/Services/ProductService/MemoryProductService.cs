using Microsoft.AspNetCore.Mvc;
using OSS.UI.Services.CategoryService;
using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;

namespace OSS.UI.Services.ProductService
{
    //GetProductListAsync: 
    public class MemoryProductService : IProductService
    {
        private readonly ICategoryService categoryService;
        private readonly IConfiguration _config;
        List<Product> _products;
        List<Category> _categories;

        public MemoryProductService([FromServices] IConfiguration config, ICategoryService categoryService)
        {
            _categories = categoryService.GetCategoryListAsync().Result.Data;
            _config = config;
            SetupData();
        }

        /// <summary> 
        /// Инициализация списков 
        /// </summary> 
        private void SetupData()
        {
            _products = new List<Product>
            {
                new Product {Id = 1, Name="Royal Canin Sterilised",
                    Description="сухой корм, для пожилых, основной компонент: злаки, образ жизни: домашний, для стерилизованных",
                    Price =53, Image="Images/1.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("dry-food")).Id},
                new Product { Id = 2, Name="Acana Pacifica CAT",
                    Description="сухой корм, для котят, для взрослых, для пожилых, основной компонент: рыба, образ жизни: домашний, беззерновой",
                    Price =92, Image="Images/2.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("dry-food")).Id},
                new Product { Id = 3, Name="Cennamo Prestige Sterilized Cat",
                    Description="Сухой корм для взрослых стерилизованных кошек с сельдью",
                    Price =36, Image="Images/3.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("dry-food")).Id},
                new Product { Id = 4, Name="Monge BWild Cat Hare",
                    Description="сухой корм, для взрослых, основной компонент: зайчатина, образ жизни: домашний",
                    Price =284, Image="Images/4.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("dry-food")).Id},
                new Product { Id = 5, Name="Purina Pro Plan Original Kitten",
                    Description="Сухой корм для котят с курицей",
                    Price =55, Image="Images/5.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("dry-food")).Id},                
                new Product { Id = 6, Name="Лежанка Эстрада Цап-царап",
                    Description="Мягкая лежанка для кошек и собак",
                    Price =31, Image="Images/6.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("houses-and-beds")).Id},
                new Product { Id = 7, Name="Banditos Кусочки в желе для кошек (Сочный ягненок)",
                    Description="Влажный корм для кошек, ягненок",
                    Price =2, Image="Images/7.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("canned-food")).Id},
                new Product { Id = 8, Name="Prochoice Паштет для кошек (Сардины и анчоусы)",
                    Description="Влажный корм для взрослых кошек",
                    Price =6, Image="Images/8.png",
                    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("canned-food")).Id},
                //new Product { Id = 9, Name="Triol Набор игрушек XW7012 для кошек (3 мыши)",
                //    Description="Набор из трех разноцветных мышек",
                //    Price =5, Image="Images/9.png",
                //    CategoryId= _categories.Find(c=>c.NormalizedName.Equals("toys")).Id},

            };
        }

        //public Task<ResponseData<ProductListModel<Product>>>
        //GetProductListAsync(
        //string? categoryNormalizedName,
        //int pageNo = 1)
        //{
        //    var model = new ProductListModel<Product>() { Items = _products };
        //    var result = new ResponseData<ProductListModel<Product>>()
        //    {
        //        Data = model
        //    };
        //    return Task.FromResult(result);
        //}

        public Task<ResponseData<ProductListModel<Product>>>
        GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {       
            // Создать объект результата 
            var result = new ResponseData<ProductListModel<Product>>();
                    // Id категории для фильрации 
                    int? categoryId = null;

                    // если требуется фильтрация, то найти Id категории 
                    // с заданным categoryNormalizedName 
                    if (categoryNormalizedName != null)
                        categoryId = _categories
                        .Find(c =>
                c.NormalizedName.Equals(categoryNormalizedName))
                         ?.Id;

                    // Выбрать объекты, отфильтрованные по Id категории, 
                    // если этот Id имеется 
                    var data = _products
                        .Where(d => categoryId == null ||
                d.CategoryId.Equals(categoryId))?
                        .ToList();
            //****************
            //// поместить ранные в объект результата 
            //result.Data = new ProductListModel<Product>() { Items = data };
            //*********************

            // получить размер страницы из конфигурации 
            int pageSize = _config.GetSection("ItemsPerPage").Get<int>();
            // получить общее количество страниц 
            int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);

            // Cоздать модель для отображения списка
            var listData = new ProductListModel<Product>()
            {
                Items = data.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            // поместить данные в объект результата 
            result.Data = listData;

            // Если список пустой 
            if (data.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбраннной категории";
            }
            // Вернуть результат 
            return Task.FromResult(result);

        }


        public Task<ResponseData<Product>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, Product product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseData<Product>> CreateProductAsync(Product product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }
}
