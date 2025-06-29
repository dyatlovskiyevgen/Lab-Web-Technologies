using Microsoft.AspNetCore.WebUtilities;
using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;
using System.Net.Http;
using System.Text.Json;

namespace OSS.UI.Services.ProductService
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiProductService> _logger;

        public ApiProductService(HttpClient httpClient, ILogger<ApiProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ResponseData<Product>> CreateProductAsync(Product product, IFormFile? formFile)
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Подготовить объект, возвращаемый методом 
            var responseData = new ResponseData<Product>();

            // Послать запрос к API для сохранения объекта 
            var response = await _httpClient.PostAsJsonAsync("api/ProductsAPI", product);

            if (!response.IsSuccessStatusCode)
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Не удалось создать объект: { response.StatusCode}"; 
                return responseData;                    
            }
            // Если файл изображения передан клиентом 
            if (formFile != null)
            {
                // получить созданный объект из ответа Api-сервиса 
                product = await response.Content.ReadFromJsonAsync<Product>();
                // создать объект запроса 
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    //RequestUri = new Uri($"{_httpClient.BaseAddress.AbsoluteUri}/{product.Id}")
                    RequestUri = new Uri($"{_httpClient.BaseAddress.AbsoluteUri}api/ProductsAPI/{product.Id}")

                };

                // Создать контент типа multipart form-data 
                var content = new MultipartFormDataContent();
                // создать потоковый контент из переданного файла 
                var streamContent = new StreamContent(formFile.OpenReadStream());
                // добавить потоковый контент в общий контент по именем "image" 
                content.Add(streamContent, "image", formFile.FileName);
                // поместить контент в запрос 
                request.Content = content;
                // послать запрос к Api-сервису 
                response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = $"Не удалось сохранить изображение: { response.StatusCode}"; 
                }
            }
            return responseData;
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<Product>> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/ProductsAPI/{id}");
                response.EnsureSuccessStatusCode();

                var product = await response.Content.ReadFromJsonAsync<Product>();
                return new ResponseData<Product>
                {
                    Data = product,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<Product>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }


        public async Task<ResponseData<ProductListModel<Product>>> GetProductListAsync(
            string? categoryNormalizedName,
            int pageNo = 1)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                    ["pageNo"] = pageNo.ToString()
                };

                if (!string.IsNullOrEmpty(categoryNormalizedName))
                {
                    query["category"] = categoryNormalizedName;
                }

                var url = QueryHelpers.AddQueryString("api/ProductsAPI", query);
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ResponseData<ProductListModel<Product>>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return new ResponseData<ProductListModel<Product>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public Task UpdateProductAsync(int id, Product product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }
    }

}
