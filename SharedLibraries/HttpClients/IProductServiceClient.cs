namespace SharedLibraries.HttpClients
{
    public interface IProductServiceClient
    {
        Task<ProductClientDto?> GetProductAsync(Guid productId);
        Task<List<ProductClientDto>> GetProductsAsync(List<Guid> productIds);
        Task<bool> CheckInventoryAsync(Guid productId, int quantity);
        Task<bool> ReserveInventoryAsync(Guid productId, int quantity, Guid orderId);
        Task<bool> ReleaseInventoryAsync(Guid productId, int quantity, Guid orderId);
        Task<ProductInventoryDto?> GetInventoryAsync(Guid productId);
    }
    
    public class ProductClientDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
        public string? BrandName { get; set; }
        public List<string> Categories { get; set; } = new();
    }
    
    public class ProductInventoryDto
    {
        public Guid ProductId { get; set; }
        public int AvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public bool IsAvailable { get; set; }
    }
    
    public class ReserveInventoryRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
    }
}
