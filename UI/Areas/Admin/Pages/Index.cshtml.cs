using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OSS.UI.Data;
using OSS.UI.Services.ProductService;
using OSS30333.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
           _productService = productService;
        }

        public IList<Product> Product { get;set; } = default!;

        //public async Task OnGetAsync()
        //{
        //    var data = await _productService.GetProductListAsync(null);
        //    Product = data.Data.Items;
        //}

        // Добавьте эти свойства для пагинации
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }

        public async Task OnGetAsync(int? pageNo = 1)
        {
            var response = await _productService.GetProductListAsync(null, pageNo.Value);
            if (response.Success)
            {
                Product = response.Data.Items;
                CurrentPage = response.Data.CurrentPage;
                TotalPages = response.Data.TotalPages;
            }
        }


        //[BindProperty(SupportsGet = true)]
        //public string? Category { get; set; }

        //public async Task OnGetAsync()
        //{
        //    var data = await _productService.GetProductListAsync(Category, CurrentPage);
        //    Product = data.Data.Items;

        //    // Установите TotalPages из ответа сервиса
        //    TotalPages = data.Data.TotalPages;
        //}



    }
}
