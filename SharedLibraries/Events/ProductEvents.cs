namespace SharedLibraries.Events
{
    public class ProductUpdatedEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class ProductCreatedEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class ProductDeletedEvent
    {
        public Guid ProductId { get; set; }
        public string ProductSku { get; set; } = string.Empty;
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class InventoryReservedEvent
    {
        public Guid ProductId { get; set; }
        public int ReservedQuantity { get; set; }
        public Guid OrderId { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    }
    
    public class InventoryReleasedEvent
    {
        public Guid ProductId { get; set; }
        public int ReleasedQuantity { get; set; }
        public Guid OrderId { get; set; }
        public DateTime ReleasedAt { get; set; } = DateTime.UtcNow;
    }
}
