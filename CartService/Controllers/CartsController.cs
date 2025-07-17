using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CartService.Models;
using CartService.Services;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly CartService.Services.CartService _cartService;
        private readonly ILogger<CartsController> _logger;

        public CartsController(CartService.Services.CartService cartService, ILogger<CartsController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartDto createCartDto)
        {
            try
            {
                var cartResponse = await _cartService.CreateCartAsync(createCartDto);
                return CreatedAtAction(nameof(GetCartById), new { id = cartResponse.Id }, cartResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCartById(Guid id)
        {
            try
            {
                var cart = await _cartService.GetCartByIdAsync(id);
                return Ok(cart);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/items")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem(Guid id, [FromBody] UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                var result = await _cartService.AddOrUpdateCartItemAsync(id, updateCartItemDto);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("items/{itemId}")]
        [Authorize]
        public async Task<IActionResult> RemoveCartItem(Guid itemId)
        {
            try
            {
                var result = await _cartService.RemoveCartItemAsync(itemId);
                if (!result)
                    return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
