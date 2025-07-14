using ProductService.Data.Common;

namespace ProductService.Data.Models
{
    public class Tag : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }

}
