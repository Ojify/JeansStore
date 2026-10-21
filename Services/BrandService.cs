using JeansStoreManagementSystem.Data;
using JeansStoreManagementSystem.DTOs.Brands;
using JeansStoreManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace JeansStoreManagementSystem.Services
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllAsync();
        Task<BrandDto> CreateAsync(CreateBrandDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class BrandService : IBrandService
    {
        private readonly ApplicationDbContext _context;
        public BrandService(ApplicationDbContext context) => _context = context;

        public async Task<List<BrandDto>> GetAllAsync()
            => await _context.Brands
                .Select(b => new BrandDto { Id = b.Id, Name = b.Name, Country = b.Country })
                .ToListAsync();

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = new Brand { Name = dto.Name, Country = dto.Country };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return new BrandDto { Id = brand.Id, Name = brand.Name, Country = brand.Country };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
