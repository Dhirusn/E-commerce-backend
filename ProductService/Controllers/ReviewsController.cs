using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.Models;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ReviewsController(ProductDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(Guid productId)
        {
            var reviews = await _context.ProductReviews
                .Where(r => r.ProductId == productId).ToListAsync();
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(Guid productId, [FromBody] ProductReview input)
        {
            var userId = User.FindFirst("sub")?.Value ?? throw new UnauthorizedAccessException();

            var review = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = input.Rating,
                Comment = input.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReviews), new { productId }, review);
        }
    }
}
