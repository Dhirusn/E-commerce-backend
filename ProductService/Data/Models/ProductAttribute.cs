using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class ProductAttribute : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
