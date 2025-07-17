using CartService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace CartService.Data.Models
{
    public class Cart : BaseEntity<Guid>
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        
        public CartStatus Status { get; set; } = CartStatus.Active;
        
        public decimal TotalAmount { get; set; }
        
        public int TotalItems { get; set; }
        
        public string? Notes { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        public string? CouponCode { get; set; }
        
        public decimal DiscountAmount { get; set; }
        
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
    
    public enum CartStatus
    {
        Active = 0,
        Abandoned = 1,
        Converted = 2, // Converted to order
        Expired = 3
    }
}
