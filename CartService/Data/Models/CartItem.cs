using CartService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace CartService.Data.Models
{
    public class CartItem : BaseEntity<Guid>
    {
        [Required]
        public Guid CartId { get; set; }
        
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
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        public string? ProductImageUrl { get; set; }
        
        public string? ProductAttributes { get; set; } // JSON string for size, color, etc.
        
        public int? AvailableStock { get; set; } // Cached from ProductService
        
        public bool IsAvailable { get; set; } = true;
        
        // Navigation properties
        public Cart Cart { get; set; } = null!;
    }
}
