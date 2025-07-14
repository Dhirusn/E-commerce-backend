using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.Models;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public SearchController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpGet("products")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var result = await _context.Products
                .Where(p => p.Title.Contains(query) || p.Description.Contains(query))
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Price,
                    p.Description,
                    // Category = p.Category.Name
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            //if (categoryId.HasValue)
            //    query = query.Where(p => p.CategoryId == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            var filtered = await query
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Price,
                    // Category = p.Category.Name
                })
                .ToListAsync();

            return Ok(filtered);
        }
    }

}
