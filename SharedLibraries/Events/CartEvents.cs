namespace SharedLibraries.Events
{
    public class CartCreatedEvent
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CartItemAddedEvent
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CartItemUpdatedEvent
    {
        public Guid CartId { get; set; }
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal NewTotalPrice { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CartItemRemovedEvent
    {
        public Guid CartId { get; set; }
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductSku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime RemovedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CartAbandonedEvent
    {
        public Guid CartId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime AbandonedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class CartConvertedEvent
    {
        public Guid CartId { get; set; }
        public Guid OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime ConvertedAt { get; set; } = DateTime.UtcNow;
    }
}
