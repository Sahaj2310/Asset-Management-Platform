using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetWeb.DTOs;
using AssetWeb.Services;
using AutoMapper;

namespace AssetWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly ILogger<LocationController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public LocationController(
            ILocationService locationService,
            ILogger<LocationController> logger,
            IMapper mapper,
            IAuthService authService)
        {
            _locationService = locationService;
            _logger = logger;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocationById(Guid id)
        {
            var (success, message, location) = await _locationService.GetLocationByIdAsync(id);
            if (!success)
            {
                return NotFound(message);
            }
            return Ok(location);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations()
        {
            var currentUserId = _authService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized("User not authenticated");
            }

            // In a real application, you might filter locations by user's company
            // For simplicity, we'll return all locations for now or by company if available
            var (success, message, locations) = await _locationService.GetAllLocationsAsync();
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(locations);
        }

        [HttpGet("site/{siteId}")]
        public async Task<IActionResult> GetLocationsBySiteId(Guid siteId)
        {
            var (success, message, locations) = await _locationService.GetLocationsBySiteIdAsync(siteId);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(locations);
        }

        [HttpGet("company/{companyId}")]
        public async Task<IActionResult> GetLocationsByCompanyId(Guid companyId)
        {
            var (success, message, locations) = await _locationService.GetLocationsByCompanyIdAsync(companyId);
            if (!success)
            {
                return BadRequest(message);
            }
            return Ok(locations);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request)
        {
            var currentUserId = _authService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized("User not authenticated");
            }

            // Additional validation if needed, e.g., check if the user belongs to the company

            var (success, message, location) = await _locationService.CreateLocationAsync(request);
            if (!success)
            {
                return BadRequest(message);
            }
            return CreatedAtAction(nameof(GetLocationById), new { id = location!.Id }, location);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] CreateLocationRequest request)
        {
            var currentUserId = _authService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized("User not authenticated");
            }

            var (success, message) = await _locationService.UpdateLocationAsync(id, request);
            if (!success)
            {
                return BadRequest(message);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(Guid id)
        {
            var currentUserId = _authService.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Unauthorized("User not authenticated");
            }

            var (success, message) = await _locationService.DeleteLocationAsync(id);
            if (!success)
            {
                return BadRequest(message);
            }
            return NoContent();
        }
    }
} 