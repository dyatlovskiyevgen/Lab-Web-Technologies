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
    public class DeleteModel : PageModel
    {
        private readonly IProductService _productService;

        public DeleteModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        //public async Task<IActionResult> OnGetAsync(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

        //    if (product is not null)
        //    {
        //        Product = product;

        //        return Page();
        //    }

        //    return NotFound();
        //}

        //public async Task<IActionResult> OnPostAsync(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        Product = product;
        //        _context.Products.Remove(Product);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToPage("./Index");
        //}
    }
}
