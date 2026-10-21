namespace JeansStoreManagementSystem.DTOs.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
