using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AssetWeb.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using AssetWeb.Data;

namespace AssetWeb.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly ApplicationDbContext _context;

        public JwtService(
            IConfiguration configuration, 
            ILogger<JwtService> logger,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(User user)
        {
            try
            {
                _logger.LogInformation("Generating tokens for user: {Email}", user.Email);
                
                var accessToken = GenerateAccessToken(user);
                var refreshToken = await GenerateRefreshTokenAsync(user);

                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tokens for user: {Email}", user.Email);
                throw;
            }
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration"));
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"] ?? "15")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            var token = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7")),
                IsRevoked = false
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<(bool Success, string? AccessToken, string? RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Token == refreshToken);

                if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
                {
                    return (false, null, null);
                }

                // Generate new tokens
                var (newAccessToken, newRefreshToken) = await GenerateTokensAsync(token.User);

                // Revoke the old refresh token
                token.IsRevoked = true;
                token.ReplacedByToken = newRefreshToken;
                token.ReasonRevoked = "Replaced by new token";
                await _context.SaveChangesAsync();

                return (true, newAccessToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return (false, null, null);
            }
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
                token.ReasonRevoked = "Revoked by user";
                await _context.SaveChangesAsync();
            }
        }
    }
} 