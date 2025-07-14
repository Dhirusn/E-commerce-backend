using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class ProductReview : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string UserId { get; set; } = string.Empty; // Use Auth0 Sub
        public int Rating { get; set; } // 1 to 5
        public string? Comment { get; set; }
    }
}
