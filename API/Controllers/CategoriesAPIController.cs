﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OSS30333.API.Data;
using OSS30333.Domain.Entities;
using OSS30333.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesAPIController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesAPIController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories 
        //[HttpGet]
        //public async Task<ActionResult<ResponseData<IEnumerable<Category>>>> GetCategories()
        //{
        //    var response = new ResponseData<IEnumerable<Category>>
        //    {
        //        Data = await _context.Categories.ToListAsync()
        //    };
        //    return response;
        //}

        [HttpGet]
        public async Task<ActionResult<ResponseData<IEnumerable<Category>>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return new ResponseData<IEnumerable<Category>> { Data = categories };
        }


            // GET: api/CategoriesAPI/5
            [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/CategoriesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CategoriesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/CategoriesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

