using OrderService.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models
{
    public class CreateOrderDto
    {
        [Required]
        public List<OrderItemDto> OrderItems { get; set; } = new();
        
        [Required]
        public ShippingAddressDto ShippingAddress { get; set; } = new();
        
        [Required]
        public BillingAddressDto BillingAddress { get; set; } = new();
        
        public string? Notes { get; set; }
        
        public decimal TaxAmount { get; set; }
        
        public decimal ShippingAmount { get; set; }
        
        public decimal DiscountAmount { get; set; }
        
        public string Currency { get; set; } = "USD";
    }
    
    public class OrderItemDto
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
    
    public class ShippingAddressDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public string AddressLine1 { get; set; } = string.Empty;
        
        public string? AddressLine2 { get; set; }
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        public string State { get; set; } = string.Empty;
        
        [Required]
        public string PostalCode { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
    }
    
    public class BillingAddressDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public string AddressLine1 { get; set; } = string.Empty;
        
        public string? AddressLine2 { get; set; }
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        public string State { get; set; } = string.Empty;
        
        [Required]
        public string PostalCode { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
    }
    
    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public ShippingAddressDto ShippingAddress { get; set; } = new();
        public BillingAddressDto BillingAddress { get; set; } = new();
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? TrackingNumber { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
    }
    
    public class OrderItemResponseDto
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
    }
    
    public class OrderStatusHistoryDto
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangedBy { get; set; }
        public string? Notes { get; set; }
    }
    
    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
        
        public string? Notes { get; set; }
        
        public string? TrackingNumber { get; set; }
    }
    
    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }
}
