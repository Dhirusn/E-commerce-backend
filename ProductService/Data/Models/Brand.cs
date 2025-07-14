using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class Brand : BaseEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
