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
            var (success, message, token, user) = await _authService.RegisterAsync(request);
            var response = new AuthResponse
            {
                Success = success,
                Message = message,
                Token = token ?? string.Empty,
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
            var (success, message, token, user) = await _authService.LoginAsync(request);
            if (!success)
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = message,
                    Token = string.Empty,
                    HasCompany = false
                });
            }

            // Check if user has a company
            var hasCompany = await _companyService.UserHasCompanyAsync(user!.Id);

            var response = new AuthResponse
            {
                Success = true,
                Message = message,
                Token = token ?? string.Empty,
                HasCompany = hasCompany
            };

            return Ok(response);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            var (success, message) = await _authService.ConfirmEmailAsync(userId, token);
            var response = new AuthResponse
            {
                Success = success,
                Message = message,
                Token = string.Empty,
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
