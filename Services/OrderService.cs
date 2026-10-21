using JeansStoreManagementSystem.Data;
using JeansStoreManagementSystem.DTOs.Orders;
using JeansStoreManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace JeansStoreManagementSystem.Services
{
    public interface IOrderService
    {
        Task<OrderDto> CreateAsync(string userId, CreateOrderDto dto);
        Task<List<OrderDto>> GetUserOrdersAsync(string userId);
        Task<List<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<bool> UpdateStatusAsync(int id, UpdateOrderStatusDto dto);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context) => _context = context;

        public async Task<OrderDto> CreateAsync(string userId, CreateOrderDto dto)
        {
            var productIds = dto.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != productIds.Count)
                throw new InvalidOperationException("Один или несколько товаров не найдены.");

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                Comment = dto.Comment,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            };

            decimal total = 0;
            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Недостаточно товара '{product.Name}' на складе.");

                product.Stock -= item.Quantity;

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                total += product.Price * item.Quantity;
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetOrderDtoAsync(order.Id);
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
            => await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => Map(o))
                .ToListAsync();

        public async Task<List<OrderDto>> GetAllAsync()
            => await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => Map(o))
                .ToListAsync();

        public async Task<OrderDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;
            if (!isAdmin && order.UserId != userId) return null;

            return Map(order);
        }

        public async Task<bool> UpdateStatusAsync(int id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
                throw new ArgumentException("Недопустимый статус заказа.");

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<OrderDto> GetOrderDtoAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .FirstAsync(o => o.Id == id);
            return Map(order);
        }

        private static OrderDto Map(Order o) => new()
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            ShippingAddress = o.ShippingAddress,
            Comment = o.Comment,
            UserEmail = o.User?.Email ?? string.Empty,
            Items = o.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}
