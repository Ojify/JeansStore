using JeansStoreManagementSystem.DTOs.Auth;
using JeansStoreManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JeansStoreManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var (success, errors, data) = await _authService.RegisterAsync(dto);
            if (!success) return BadRequest(new { errors });
            return Ok(data);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (success, error, data) = await _authService.LoginAsync(dto);
            if (!success) return Unauthorized(new { error });
            return Ok(data);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
                Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
            });
        }
    }
}
