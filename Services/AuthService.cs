using JeansStoreManagementSystem.Constants;
using JeansStoreManagementSystem.DTOs.Auth;
using JeansStoreManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace JeansStoreManagementSystem.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string[] Errors, AuthResponseDto? Data)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Error, AuthResponseDto? Data)> LoginAsync(LoginDto dto);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IConfiguration config)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
        }

        public async Task<(bool Success, string[] Errors, AuthResponseDto? Data)> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return (false, new[] { "Пользователь с таким email уже существует." }, null);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray(), null);

            await _userManager.AddToRoleAsync(user, Roles.User);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);
            var expiresHours = double.Parse(_config["Jwt:ExpiresHours"] ?? "24");

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                UserId = user.Id,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(expiresHours)
            };

            return (true, Array.Empty<string>(), response);
        }

        public async Task<(bool Success, string Error, AuthResponseDto? Data)> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return (false, "Неверный email или пароль.", null);

            var passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordOk)
                return (false, "Неверный email или пароль.", null);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);
            var expiresHours = double.Parse(_config["Jwt:ExpiresHours"] ?? "24");

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                UserId = user.Id,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddHours(expiresHours)
            };

            return (true, string.Empty, response);
        }
    }
}
