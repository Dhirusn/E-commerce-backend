namespace SharedLibraries.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid? CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public List<OrderItemEvent> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class OrderItemEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
    public class OrderStatusChangedEvent
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class OrderCancelledEvent
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public List<OrderItemEvent> Items { get; set; } = new();
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
    }
    
    public class OrderShippedEvent
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public DateTime ShippedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class OrderDeliveredEvent
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime DeliveredAt { get; set; } = DateTime.UtcNow;
    }
}
