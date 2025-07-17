using OrderService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models
{
    public class OrderItem : BaseEntity<Guid>
    {
        [Required]
        public Guid OrderId { get; set; }
        
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
        
        // Navigation properties
        public Order Order { get; set; } = null!;
    }
}
