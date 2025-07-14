using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.Models;
using ProductService.Shared;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public CategoriesController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<Result<PaginatedResult<Category>>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Categories.AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var res = new PaginatedResult<Category>(items, totalCount, pageNumber, pageSize);
            return Result<PaginatedResult<Category>>.Ok(res, "");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _context.Categories.Include(x=>x.Children).FirstOrDefaultAsync(x => x.Id == id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("parent/{parentId}")]
        public async Task<IActionResult> GetByParentId(Guid parentId)
        {
            var children = await _context.Categories
                .Where(c => c.ParentId == parentId)
                .ToListAsync();
            return Ok(children);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Category updated)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = updated.Name;
            category.ParentId = updated.ParentId;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
