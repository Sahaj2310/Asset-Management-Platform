using System.Threading.Tasks;
using AssetWeb.DTOs;
using AssetWeb.Models;

namespace AssetWeb.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token, User? User)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(LoginRequest request);
        Task<User?> GetUserByIdAsync(Guid userId);
        Guid? GetCurrentUserId();
        Task<(bool Success, string Message)> ConfirmEmailAsync(Guid userId, string token);
        Task<bool> UserExists(string email);
        Task<bool> UpdateUserAsync(User user);
    }
} 