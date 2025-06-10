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
using Microsoft.AspNetCore.Authentication;

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
        private readonly JwtService _jwtService;

        public AuthController(
            IAuthService authService,
            ICompanyService companyService,
            IMapper mapper,
            ILogger<AuthController> logger,
            JwtService jwtService)
        {
            _authService = authService;
            _companyService = companyService;
            _mapper = mapper;
            _logger = logger;
            _jwtService = jwtService;
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

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback"),
                Items =
                {
                    { "scheme", "Google" }
                }
            };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync("Google");
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Google authentication failed");
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Google authentication failed",
                        AccessToken = string.Empty,
                        RefreshToken = string.Empty,
                        HasCompany = false
                    });
                }

                var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email not found in Google account");
                    return BadRequest(new AuthResponse
                    {
                        Success = false,
                        Message = "Email not found in Google account",
                        AccessToken = string.Empty,
                        RefreshToken = string.Empty,
                        HasCompany = false
                    });
                }

                var user = await _authService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    // Create new user from Google account
                    var firstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
                    var lastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
                    
                    var registerRequest = new RegisterRequest
                    {
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        Password = Guid.NewGuid().ToString(), // Generate random password
                        ConfirmPassword = Guid.NewGuid().ToString(),
                        Role = "User"
                    };

                    var (success, message, _, _, newUser) = await _authService.RegisterAsync(registerRequest);
                    if (!success)
                    {
                        _logger.LogWarning("Failed to register user from Google account: {Message}", message);
                        return BadRequest(new AuthResponse
                        {
                            Success = false,
                            Message = message,
                            AccessToken = string.Empty,
                            RefreshToken = string.Empty,
                            HasCompany = false
                        });
                    }

                    user = newUser;
                }

                // Generate our own JWT tokens
                var (accessToken, refreshToken) = await _jwtService.GenerateTokensAsync(user);
                var hasCompany = await _companyService.UserHasCompanyAsync(user.Id);

                _logger.LogInformation("Google authentication successful for user: {Email}", email);

                return Ok(new AuthResponse
                {
                    Success = true,
                    Message = "Google login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    HasCompany = hasCompany
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google authentication");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during authentication",
                    AccessToken = string.Empty,
                    RefreshToken = string.Empty,
                    HasCompany = false
                });
            }
        }
    }
}
