namespace JeansStoreManagementSystem.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Country { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
