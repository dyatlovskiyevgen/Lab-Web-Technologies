using Microsoft.AspNetCore.Http;
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
    public class ProductsAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;//***            

        public ProductsAPIController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;//***
        }

        public async Task<ActionResult<ResponseData<ProductListModel<Product>>>> GetProducts(
        string? category = null,
        int pageNo = 1,
        int pageSize = 3)
        {
            try
            {
                // Валидация параметров
                if (pageNo < 1) pageNo = 1;
                if (pageSize < 1) pageSize = 3;

                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Фильтрация по категории (если указана)
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(p =>
                        p.Category.NormalizedName.ToLower() == category.ToLower());
                }

                // Пагинация
                int totalItems = await query.CountAsync();
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                if (pageNo > totalPages && totalPages > 0)
                    pageNo = totalPages;

                var items = await query
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var result = new ResponseData<ProductListModel<Product>>
                {
                    Data = new ProductListModel<Product>
                    {
                        Items = items,
                        CurrentPage = pageNo,
                        TotalPages = totalPages
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
                return StatusCode(500, new ResponseData<ProductListModel<Product>>
                {
                    Success = false,
                    ErrorMessage = "Internal Server Error"
                });
            }
        }

        // GET: api/ProductsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/ProductsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/ProductsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }


        [HttpPost("{id}")]
        public async Task<IActionResult> SaveImage(int id, IFormFile image)
        {
            // Найти объект по Id 
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Путь к папке wwwroot/Images 
            var imagesPath = Path.Combine(_env.WebRootPath, "Images");
            // получить случайное имя файла 
            var randomName = Path.GetRandomFileName();
            // получить расширение в исходном файле 
            var extension = Path.GetExtension(image.FileName);
            // задать в новом имени расширение как в исходном файле 
            var fileName = Path.ChangeExtension(randomName, extension);
            // полный путь к файлу 
            var filePath = Path.Combine(imagesPath, fileName);
            // создать файл и открыть поток для записи 
            using var stream = System.IO.File.OpenWrite(filePath);
            // скопировать файл в поток 
            await image.CopyToAsync(stream);
            // получить Url хоста 
            var host = "https://" + Request.Host;
            // Url файла изображения 
            var url = $"{host}/Images/{fileName}";
            // Сохранить url файла в объекте 
            product.Image = url;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/ProductsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

