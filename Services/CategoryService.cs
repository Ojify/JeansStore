using JeansStoreManagementSystem.Data;
using JeansStoreManagementSystem.DTOs.Categories;
using JeansStoreManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace JeansStoreManagementSystem.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context) => _context = context;

        public async Task<List<CategoryDto>> GetAllAsync()
            => await _context.Categories
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description })
                .ToListAsync();

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category { Name = dto.Name, Description = dto.Description };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryDto { Id = category.Id, Name = category.Name, Description = category.Description };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
