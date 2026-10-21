using JeansStoreManagementSystem.Data;
using JeansStoreManagementSystem.DTOs.Products;
using JeansStoreManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace JeansStoreManagementSystem.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(string? category, string? brand, string? search);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context) => _context = context;

        public async Task<List<ProductDto>> GetAllAsync(string? category, string? brand, string? search)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.Category.Name == category);

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(p => p.Brand.Name == brand);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) ||
                                         p.Description.Contains(search));

            return await query.Select(p => Map(p)).ToListAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .FirstOrDefaultAsync(x => x.Id == id);

            return p == null ? null : Map(p);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                Size = dto.Size,
                Color = dto.Color,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.Brand).LoadAsync();

            return Map(product);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Size = dto.Size;
            product.Color = dto.Color;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;
            product.BrandId = dto.BrandId;

            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();
            await _context.Entry(product).Reference(p => p.Brand).LoadAsync();

            return Map(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ProductDto Map(Product p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            Size = p.Size,
            Color = p.Color,
            ImageUrl = p.ImageUrl,
            CategoryName = p.Category?.Name ?? string.Empty,
            BrandName = p.Brand?.Name ?? string.Empty
        };
    }
}
