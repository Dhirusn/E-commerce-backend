using System.ComponentModel.DataAnnotations;
using CartService.Data.Models;

namespace CartService.Models
{
    public class CreateCartDto
    {
        [Required]
        public List<CartItemDto> CartItems { get; set; } = new();
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
        
        public string? CouponCode { get; set; }
    }
    
    public class CartItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public string ProductName { get; set; } = string.Empty;
        
        [Required]
        public string ProductSku { get; set; } = string.Empty;
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        public string? ProductImageUrl { get; set; }
        
        public string? ProductAttributes { get; set; }
    }
    
    public class CartResponseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public CartStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public string? Notes { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public List<CartItemResponseDto> CartItems { get; set; } = new();
    }
    
    public class CartItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ProductImageUrl { get; set; }
        public string? ProductAttributes { get; set; }
        public int? AvailableStock { get; set; } // Cached from ProductService
        public bool IsAvailable { get; set; } = true;
    }
    
    public class UpdateCartItemDto
    {
        [Required]
        public Guid CartItemId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
    }

    public class CartSummaryDto
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
