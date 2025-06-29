using OSS30333.Domain.Models;
using OSS30333.Domain.Entities;

namespace OSS.UI.Services.CategoryService
{
    public interface ICategoryService
    {
        /// <summary> 
        /// Получение списка всех категорий 
        /// </summary> 
        /// <returns></returns> 
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}
