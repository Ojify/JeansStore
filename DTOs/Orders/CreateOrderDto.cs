namespace JeansStoreManagementSystem.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
