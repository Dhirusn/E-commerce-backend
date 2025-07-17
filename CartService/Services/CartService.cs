using CartService.Data;
using CartService.Data.Models;
using CartService.Models;
using Microsoft.EntityFrameworkCore;
using SharedLibraries.UserServices;
using SharedLibraries.EventBus;
using SharedLibraries.Events;
using SharedLibraries.HttpClients;

namespace CartService.Services
{
    public class CartService
    {
        private readonly CartDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<CartService> _logger;

        public CartService(CartDbContext context, IUserContextService userContextService, ILogger<CartService> logger)
        {
            _context = context;
            _userContextService = userContextService;
            _logger = logger;
        }

        public async Task<CartResponseDto> CreateCartAsync(CreateCartDto createCartDto)
        {
            try
            {
                var userId = _userContextService.UserId;
                var userEmail = _userContextService.Email;

                var cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UserEmail = userEmail,
                    Status = CartStatus.Active,
                    Notes = createCartDto.Notes,
                    CouponCode = createCartDto.CouponCode,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var itemDto in createCartDto.CartItems)
                {
                    var cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = itemDto.ProductId,
                        ProductName = itemDto.ProductName,
                        ProductSku = itemDto.ProductSku,
                        UnitPrice = itemDto.UnitPrice,
                        Quantity = itemDto.Quantity,
                        TotalPrice = itemDto.UnitPrice * itemDto.Quantity,
                        ProductImageUrl = itemDto.ProductImageUrl,
                        ProductAttributes = itemDto.ProductAttributes,
                        CreatedAt = DateTime.UtcNow
                    };

                    cart.TotalAmount += cartItem.TotalPrice;
                    cart.TotalItems += cartItem.Quantity;

                    _context.CartItems.Add(cartItem);
                }

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                return await GetCartByIdAsync(cart.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart");
                throw;
            }
        }

        public async Task<bool> AddOrUpdateCartItemAsync(Guid cartId, UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == updateCartItemDto.CartItemId);

                if (cartItem == null)
                    return false;

                cartItem.Quantity = updateCartItemDto.Quantity;
                cartItem.TotalPrice = cartItem.UnitPrice * updateCartItemDto.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return false;
            }
        }

        public async Task<CartResponseDto> GetCartByIdAsync(Guid cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId && !c.IsDeleted);

            if (cart == null)
                throw new InvalidOperationException("Cart not found");

            return MapToCartResponseDto(cart);
        }

        public async Task<bool> RemoveCartItemAsync(Guid cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId);

                if (cartItem == null)
                    return false;

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return false;
            }
        }

        private CartResponseDto MapToCartResponseDto(Cart cart)
        {
            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                UserEmail = cart.UserEmail,
                Status = cart.Status,
                TotalAmount = cart.TotalAmount,
                TotalItems = cart.TotalItems,
                Notes = cart.Notes,
                CouponCode = cart.CouponCode,
                DiscountAmount = cart.DiscountAmount,
                ExpiresAt = cart.ExpiresAt,
                CartItems = cart.CartItems.Select(item => new CartItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductSku = item.ProductSku,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    ProductImageUrl = item.ProductImageUrl,
                    ProductAttributes = item.ProductAttributes,
                    AvailableStock = item.AvailableStock,
                    IsAvailable = item.IsAvailable
                }).ToList()
            };
        }
    }
}
