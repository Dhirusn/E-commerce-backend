using OrderService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models
{
    public class OrderStatusHistory : BaseEntity<Guid>
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        
        public string? ChangedBy { get; set; }
        
        public string? Notes { get; set; }
        
        // Navigation properties
        public Order Order { get; set; } = null!;
    }
}
