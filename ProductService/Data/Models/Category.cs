using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class Category : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> Children { get; set; } = new List<Category>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    }
}
