using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetWeb.DTOs;
using AssetWeb.Services;
using AutoMapper;
using System.IO;
using ClosedXML.Excel;
using AssetWeb.Repositories;

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
        private readonly ILocationRepository _locationRepository;
        private readonly ISiteRepository _siteRepository;

        public LocationController(
            ILocationService locationService,
            ILogger<LocationController> logger,
            IMapper mapper,
            IAuthService authService,
            ILocationRepository locationRepository,
            ISiteRepository siteRepository)
        {
            _locationService = locationService;
            _logger = logger;
            _mapper = mapper;
            _authService = authService;
            _locationRepository = locationRepository;
            _siteRepository = siteRepository;
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

        [HttpGet("export-excel")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public async Task<IActionResult> ExportLocationsToExcel()
        {
            var locations = await _locationRepository.GetAllLocationsAsync();
            var sites = await _siteRepository.GetAllAsync();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Locations");

            // Add headers
            worksheet.Cell(1, 1).Value = "Location Name";
            worksheet.Cell(1, 2).Value = "Site Name";
            worksheet.Cell(1, 3).Value = "Assets Count";

            // Style the header row
            var headerRange = worksheet.Range(1, 1, 1, 3);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            
            // Add borders to header
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Add data
            int row = 2;
            foreach (var location in locations)
            {
                var site = sites.FirstOrDefault(s => s.Id == location.SiteId);
                var assetsCount = 0; // You'll need to implement this based on your asset model

                worksheet.Cell(row, 1).Value = location.Name;
                worksheet.Cell(row, 2).Value = site?.Name ?? string.Empty;
                worksheet.Cell(row, 3).Value = assetsCount;
                row++;
            }

            // Add borders to data range
            var dataRange = worksheet.Range(1, 1, row - 1, 3);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Enable filtering
            var filterRange = worksheet.Range(1, 1, row - 1, 3);
            filterRange.SetAutoFilter();

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Create memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "locations_export.xlsx"
            );
        }
    }
} 