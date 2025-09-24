using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Data.Models;
using ProductService.Models;
using ProductService.Services;
using ProductService.Shared;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productService;

        public ProductsController(ProductsService productService)
        {
            _productService = productService;
        }

        [HttpGet("getAll")]
        public async Task<Result<PaginatedResult<Product>>> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productService.GetAllAsync(pageNumber, pageSize);
            return Result<PaginatedResult<Product>>.Ok(products, null);
        }

        [HttpGet("GetById/{id}")]
        public async Task<Result<Product>> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            return product == null ? Result<Product>.Fail("not found", "err") : Result<Product>.Ok(product, null);
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto product)
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Product input)
        {
            var updated = await _productService.UpdateAsync(id, input);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}