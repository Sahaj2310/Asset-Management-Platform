using System;
using System.Threading.Tasks;
using AssetWeb.Models;
using AssetWeb.DTOs;

namespace AssetWeb.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> UserExists(string email);
        Task<User> Register(RegisterRequest userDto);
        Task<(bool Success, string Message)> ConfirmEmail(Guid userId, string token);
        Task<User?> Login(string email, string password);
        Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
    }
}
