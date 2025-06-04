using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AssetWeb.DTOs;
using AssetWeb.Models;
using AssetWeb.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AssetWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IAuthService authService,
            IMapper mapper,
            ILogger<ProfileController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = _authService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _authService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var profileDto = _mapper.Map<UserProfileResponse>(user);
                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = _authService.GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var user = await _authService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Update user properties
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                
                // Only update email if it's different and not already taken
                if (request.Email != user.Email)
                {
                    if (await _authService.UserExists(request.Email))
                    {
                        return BadRequest(new { message = "Email is already taken" });
                    }
                    user.Email = request.Email;
                    user.EmailConfirmed = false; // Require email confirmation for new email
                }

                var success = await _authService.UpdateUserAsync(user);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update profile" });
                }

                var updatedUser = await _authService.GetUserByIdAsync(userId.Value);
                var profileDto = _mapper.Map<UserProfileResponse>(updatedUser);
                return Ok(profileDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateProfile");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }
    }
} 