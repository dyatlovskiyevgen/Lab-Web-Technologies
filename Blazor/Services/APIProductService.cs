using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;

namespace OSS.Blazor.Services
{
    public class ApiProductService(HttpClient Http) : IProductService<Product>
    {
        List<Product> _products = new();
        int _currentPage = 1;
        int _totalPages = 1;
        public IEnumerable<Product> Products => _products;

        public int CurrentPage => _currentPage;
        public int TotalPages => _totalPages;

        public event Action ListChanged;

        public async Task GetProducts(int pageNo, int pageSize)
        {
            Console.WriteLine("⏳ ApiProductService.GetProducts вызван..."); //*****


            // Url сервиса API 
            var uri = Http.BaseAddress.AbsoluteUri;            

            // данные для Query запроса 
            var queryData = new Dictionary<string, string?>()
            {
            { "pageNo", pageNo.ToString() },
            {"pageSize", pageSize.ToString() }
            };

            var query = QueryString.Create(queryData);
            // Отправить запрос http 
            var result = await Http.GetAsync(uri + query.Value);
            
            var json = await result.Content.ReadAsStringAsync();
            Console.WriteLine("JSON ОТВЕТ:");
            Console.WriteLine(json);


            // В случае успешного ответа 
            if (result.IsSuccessStatusCode)
            {
                // получить данные из ответа 
                var responseData = await result.Content
                    .ReadFromJsonAsync<ResponseData<ProductListModel<Product>>>();
                // обновить параметры 
                _currentPage = responseData.Data.CurrentPage;
                _totalPages = responseData.Data.TotalPages;
                _products = responseData.Data.Items;
                ListChanged?.Invoke();
            }
            // В случае ошибки 
            else
            {
                _products = null;
                _currentPage = 1;
                _totalPages = 1;
            }
        }
    }
}
