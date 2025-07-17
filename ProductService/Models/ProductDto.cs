namespace ProductService.Models
{
    public class ProductCreateDto
    {
        public string? Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }

        public string? BrandId { get; set; }

        public List<Guid> CategoryIds { get; set; } = new();
    }


    public class ProductResponseDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Stock { get; set; }

        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }

        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CategoryDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; } = string.Empty;
    }


}
