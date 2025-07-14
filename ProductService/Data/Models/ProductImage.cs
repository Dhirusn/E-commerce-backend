using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class ProductImage : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; } = false;
    }
}
