using JeansStoreManagementSystem.DTOs.Categories;
using JeansStoreManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JeansStoreManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoriesController(ICategoryService service) => _service = service;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto) => Ok(await _service.CreateAsync(dto));

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id) => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
