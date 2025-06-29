using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OSS.UI.Data;
using OSS.UI.Services.CategoryService;
using OSS.UI.Services.ProductService;
using OSS30333.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class CreateModel(ICategoryService categoryService, IProductService productService) : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            var categoryListData = await categoryService.GetCategoryListAsync();
            ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        [BindProperty]
        public IFormFile? Image { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD 
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await productService.CreateProductAsync(Product, Image);              

        return RedirectToPage("./Index");
        }



    }
}
