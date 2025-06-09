using Microsoft.AspNetCore.Mvc;
using AssetWeb.DTOs;
using AssetWeb.Models;
using AssetWeb.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;

namespace AssetWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ICompanyService companyService,
            IMapper mapper,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _companyService = companyService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (success, message, accessToken, refreshToken, user) = await _authService.RegisterAsync(request);
            var response = new AuthResponse
            {
                Success = success,
                Message = message,
                AccessToken = accessToken ?? string.Empty,
                RefreshToken = refreshToken ?? string.Empty,
                HasCompany = false
            };

            if (!success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var (success, message, accessToken, refreshToken, user) = await _authService.LoginAsync(request);
            if (!success)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = message,
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    HasCompany = false
                });
            }

            var hasCompany = await _companyService.UserHasCompanyAsync(user!.Id);

            var response = new AuthResponse
            {
                Success = true,
                Message = message,
                AccessToken = accessToken ?? string.Empty,
                RefreshToken = refreshToken ?? string.Empty,
                HasCompany = hasCompany
            };

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var (success, message, accessToken, refreshToken) = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (!success)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = message,
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    HasCompany = false
                });
            }

            return Ok(new AuthResponse
            {
                Success = true,
                Message = message,
                AccessToken = accessToken ?? string.Empty,
                RefreshToken = refreshToken ?? string.Empty,
                HasCompany = false
            });
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
            return Ok(new { Message = "Token revoked successfully" });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var (success, message) = await _authService.ConfirmEmailAsync(userId, token);
            var response = new AuthResponse
            {
                Success = success,
                Message = message,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                HasCompany = false
            };

            if (!success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
