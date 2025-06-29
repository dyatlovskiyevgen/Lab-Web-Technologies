using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;

namespace OSS.UI.Services.CategoryService
{
    public class MemoryCetegoryService : ICategoryService
    {
        public Task<ResponseData<List<Category>>>
GetCategoryListAsync()
        {
            var categories = new List<Category>
        {
            new Category {Id=1, Name="Сухие корма",  NormalizedName="dry-food"},
            new Category {Id=2, Name="Консервы",NormalizedName="canned-food"},
            new Category {Id=3, Name="Домики и лежанки",NormalizedName="houses-and-beds"},
            new Category {Id=4, Name="Игрушки",NormalizedName="toys"}
        };
            var result = new ResponseData<List<Category>>();
            result.Data = categories;
            return Task.FromResult(result);
        }
    }
}
