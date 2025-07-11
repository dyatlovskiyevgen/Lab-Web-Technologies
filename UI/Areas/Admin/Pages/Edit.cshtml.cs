﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;

        public EditModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        //    public async Task<IActionResult> OnGetAsync(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var product =  await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }
        //        Product = product;
        //       ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        //        return Page();
        //    }

        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more information, see https://aka.ms/RazorPagesCRUD.
        //    public async Task<IActionResult> OnPostAsync()
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Page();
        //        }

        //        _context.Attach(Product).State = EntityState.Modified;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(Product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return RedirectToPage("./Index");
        //    }

        //    private bool ProductExists(int id)
        //    {
        //        return _context.Products.Any(e => e.Id == id);
        //    }
    }
}
