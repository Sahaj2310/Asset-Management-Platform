using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AssetWeb.DTOs;
using AssetWeb.Models;
using AssetWeb.Repositories;
using System.Security.Claims;

namespace AssetWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(
            IAuthRepository authRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthService> logger,
            JwtService jwtService,
            IEmailService emailService)
        {
            _authRepository = authRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Message, string? Token, User? User)> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

            if (await _authRepository.UserExists(request.Email))
            {
                _logger.LogWarning("Registration failed: Email already exists: {Email}", request.Email);
                return (false, "Email already exists", null, null);
            }

            var user = await _authRepository.Register(request);
            
            // Generate email confirmation token and send email
            var confirmationToken = await _authRepository.GenerateEmailConfirmationTokenAsync(user.Id);
            var emailSent = await _emailService.SendEmailConfirmationAsync(user.Email, user.Id, confirmationToken);
            
            if (!emailSent)
            {
                _logger.LogWarning("Failed to send confirmation email to: {Email}", request.Email);
                return (false, "Registration successful but failed to send confirmation email. Please contact support.", null, null);
            }

            _logger.LogInformation("User registered successfully and confirmation email sent: {Email}", request.Email);
            return (true, "Registration successful. Please check your email to confirm your account.", null, user);
        }

        public async Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(LoginRequest request)
        {
            var user = await _authRepository.Login(request.Email, request.Password);
            if (user == null)
            {
                return (false, "Invalid email or password", null, null);
            }

            var token = _jwtService.GenerateToken(user);
            return (true, "Login successful", token, user);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _authRepository.GetUserByIdAsync(userId);
        }

        public Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return null;
            }
            return userId;
        }

        public async Task<(bool Success, string Message)> ConfirmEmailAsync(Guid userId, string token)
        {
            return await _authRepository.ConfirmEmail(userId, token);
        }

        public async Task<bool> UserExists(string email)
        {
            return await _authRepository.UserExists(email);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _authRepository.UpdateUserAsync(user);
        }
    }
} 