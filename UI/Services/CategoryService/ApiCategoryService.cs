using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;
using System.Net.Http.Json;

namespace OSS.UI.Services.CategoryService
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/CategoriesAPI");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ResponseData<List<Category>>>();
            }
            catch (Exception ex)
            {
                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}


















//using OSS30333.Domain.Entities;
//using OSS30333.Domain.Models;

//namespace OSS.UI.Services.CategoryService
//{
//    public class ApiCategoryService(HttpClient httpClient) : ICategoryService
//    {
//        //    public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
//        //    {
//        //        var result = await httpClient.GetAsync(httpClient.BaseAddress);
//        //        if (result.IsSuccessStatusCode)
//        //        {
//        //            return await result.Content
//        //.ReadFromJsonAsync<ResponseData<List<Category>>>();
//        //        }
//        //        ;

//        //        var response = new ResponseData<List<Category>>
//        //        { Success = false, ErrorMessage = "Ошибка чтения API" };
//        //        return response;
//        //    }

//        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
//        {
//            try
//            {
//                // Указываем конкретный путь API
//                var result = await httpClient.GetAsync("/api/CategoriesAPI");

//                if (result.IsSuccessStatusCode)
//                {
//                    return await result.Content.ReadFromJsonAsync<ResponseData<List<Category>>>();
//                }

//                return new ResponseData<List<Category>>
//                {
//                    Success = false,
//                    ErrorMessage = $"Ошибка API: {result.StatusCode}"
//                };
//            }
//            catch (Exception ex)
//            {
//                return new ResponseData<List<Category>>
//                {
//                    Success = false,
//                    ErrorMessage = $"Ошибка: {ex.Message}"
//                };
//            }
//        }



//    }
//}
