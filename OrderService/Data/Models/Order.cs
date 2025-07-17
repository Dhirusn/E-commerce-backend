using OrderService.Data.Common;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Data.Models
{
    public class Order : BaseEntity<Guid>
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public decimal SubTotal { get; set; }
        
        public decimal TaxAmount { get; set; }
        
        public decimal ShippingAmount { get; set; }
        
        public decimal DiscountAmount { get; set; }
        
        [Required]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public ShippingAddress ShippingAddress { get; set; } = new();
        
        [Required]
        public BillingAddress BillingAddress { get; set; } = new();
        
        public string? Notes { get; set; }
        
        public DateTime? ShippedAt { get; set; }
        
        public DateTime? DeliveredAt { get; set; }
        
        public string? TrackingNumber { get; set; }
        
        public string? PaymentIntentId { get; set; }
        
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
    
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Refunded = 6
    }
}
