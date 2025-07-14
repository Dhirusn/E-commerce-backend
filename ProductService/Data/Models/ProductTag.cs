using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class ProductTag 
    {
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public Product Product { get; set; } = null!;

        public Guid TagId { get; set; } = Guid.NewGuid();
        public Tag Tag { get; set; } = null!;
    }
}
