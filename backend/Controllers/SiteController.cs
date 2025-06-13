using AssetWeb.Models;
using AssetWeb.DTOs;
using AssetWeb.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using AssetWeb.Services;
using ClosedXML.Excel;
using System.IO;

namespace AssetWeb.Controllers
{
    [ApiController]
    [Route("api/sites")]
    [Authorize]
    public class SiteController : ControllerBase
    {
        private readonly ISiteRepository _siteRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly LocationDataService _locationDataService;
        private readonly IAuthService _authService;

        public SiteController(
            ISiteRepository siteRepository, 
            ILocationRepository locationRepository,
            LocationDataService locationDataService,
            IAuthService authService)
        {
            _siteRepository = siteRepository;
            _locationRepository = locationRepository;
            _locationDataService = locationDataService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteReadDto>>> GetSites()
        {
            var sites = await _siteRepository.GetAllAsync();
            var locations = await _locationRepository.GetAllLocationsAsync();

            var siteDtos = sites.Select(site =>
            {
                var siteLocations = locations.Where(l => l.SiteId == site.Id).ToList();
                var country = _locationDataService.GetCountries().FirstOrDefault(c => c.Id == site.CountryId);
                var state = _locationDataService.GetStatesByCountry(site.CountryId).FirstOrDefault(s => s.Id == site.StateId);
                var city = _locationDataService.GetCitiesByState(site.StateId).FirstOrDefault(c => c.Id == site.CityId);

                return new SiteReadDto
                {
                    Id = site.Id,
                    CompanyId = site.CompanyId,
                    CompanyName = site.Company?.CompanyName ?? string.Empty,
                    Name = site.Name,
                    Description = site.Description,
                    Address = site.Address,
                    CountryId = site.CountryId,
                    CountryName = country?.Name ?? string.Empty,
                    StateId = site.StateId,
                    StateName = state?.Name ?? string.Empty,
                    CityId = site.CityId,
                    CityName = city?.Name ?? string.Empty,
                    ZipCode = site.ZipCode,
                    AssetsCount = siteLocations.Count,
                    LocationCount = siteLocations.Count
                };
            });

            return Ok(siteDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SiteReadDto>> GetSite(Guid id)
        {
            var site = await _siteRepository.GetSiteByIdAsync(id);
            if (site == null)
            {
                return NotFound();
            }

            var locations = await _locationRepository.GetLocationsBySiteIdAsync(id);
            var country = _locationDataService.GetCountries().FirstOrDefault(c => c.Id == site.CountryId);
            var state = _locationDataService.GetStatesByCountry(site.CountryId).FirstOrDefault(s => s.Id == site.StateId);
            var city = _locationDataService.GetCitiesByState(site.StateId).FirstOrDefault(c => c.Id == site.CityId);

            var siteDto = new SiteReadDto
            {
                Id = site.Id,
                CompanyId = site.CompanyId,
                CompanyName = site.Company?.CompanyName ?? string.Empty,
                Name = site.Name,
                Description = site.Description,
                Address = site.Address,
                CountryId = site.CountryId,
                CountryName = country?.Name ?? string.Empty,
                StateId = site.StateId,
                StateName = state?.Name ?? string.Empty,
                CityId = site.CityId,
                CityName = city?.Name ?? string.Empty,
                ZipCode = site.ZipCode,
                AssetsCount = locations.Count(),
                LocationCount = locations.Count()
            };

            return Ok(siteDto);
        }

        [HttpPost]
        public async Task<ActionResult<SiteReadDto>> CreateSite([FromBody] SiteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var site = new Site
            {
                CompanyId = dto.CompanyId,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address,
                CountryId = dto.CountryId,
                StateId = dto.StateId,
                CityId = dto.CityId,
                ZipCode = dto.ZipCode
            };
            await _siteRepository.AddAsync(site);
            var readDto = new SiteReadDto
            {
                Id = site.Id,
                CompanyId = site.CompanyId,
                CompanyName = site.Company != null ? site.Company.CompanyName : string.Empty,
                Name = site.Name,
                Description = site.Description,
                Address = site.Address,
                CountryId = site.CountryId,
                StateId = site.StateId,
                CityId = site.CityId,
                ZipCode = site.ZipCode
            };
            return CreatedAtAction(nameof(GetSite), new { id = site.Id }, readDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSite(Guid id, [FromBody] SiteUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var site = await _siteRepository.GetSiteByIdAsync(id);
            if (site == null) return NotFound();
            site.CompanyId = dto.CompanyId;
            site.Name = dto.Name;
            site.Description = dto.Description;
            site.Address = dto.Address;
            site.CountryId = dto.CountryId;
            site.StateId = dto.StateId;
            site.CityId = dto.CityId;
            site.ZipCode = dto.ZipCode;
            await _siteRepository.UpdateAsync(site);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSite(Guid id)
        {
            var site = await _siteRepository.GetSiteByIdAsync(id);
            if (site == null) return NotFound();
            await _siteRepository.DeleteAsync(site);
            return NoContent();
        }

        [HttpGet("export-excel")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public async Task<IActionResult> ExportSitesToExcel()
        {
            var sites = await _siteRepository.GetAllAsync();
            var locations = await _locationRepository.GetAllLocationsAsync();
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sites");

            worksheet.Cell(1, 1).Value = "Site Name";
            worksheet.Cell(1, 2).Value = "Assets Count";
            worksheet.Cell(1, 3).Value = "Location Count";
            worksheet.Cell(1, 4).Value = "City";
            worksheet.Cell(1, 5).Value = "State";

            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            int row = 2;
            foreach (var site in sites)
            {
                var siteLocations = locations.Where(l => l.SiteId == site.Id).ToList();
                var state = _locationDataService.GetStatesByCountry(site.CountryId).FirstOrDefault(s => s.Id == site.StateId);
                var city = _locationDataService.GetCitiesByState(site.StateId).FirstOrDefault(c => c.Id == site.CityId);

                worksheet.Cell(row, 1).Value = site.Name;
                worksheet.Cell(row, 2).Value = siteLocations.Count; // Assets count
                worksheet.Cell(row, 3).Value = siteLocations.Count; // Location count
                worksheet.Cell(row, 4).Value = city?.Name ?? string.Empty;
                worksheet.Cell(row, 5).Value = state?.Name ?? string.Empty;
                row++;
            }

            // Add borders to data range
            var dataRange = worksheet.Range(1, 1, row - 1, 5);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Enable filtering
            var filterRange = worksheet.Range(1, 1, row - 1, 5);
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
                "sites_export.xlsx"
            );
        }

        [HttpGet("template")]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        public IActionResult DownloadTemplate()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sites Template");

            // Add headers
            worksheet.Cell(1, 1).Value = "SiteName";
            worksheet.Cell(1, 2).Value = "Description";
            worksheet.Cell(1, 3).Value = "Address";
            worksheet.Cell(1, 4).Value = "Appartment";
            worksheet.Cell(1, 5).Value = "Country";
            worksheet.Cell(1, 6).Value = "State";
            worksheet.Cell(1, 7).Value = "City";
            worksheet.Cell(1, 8).Value = "ZipCode";

            // Style the header row
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Create memory stream
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "sites_import_template.xlsx"
            );
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportSitesFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please upload a valid Excel file");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Please upload a valid .xlsx file");
            }

            try
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1); // Get first worksheet
                var rows = worksheet.RowsUsed().Skip(1); // Skip header row

                var importedSites = new List<Site>();
                var errors = new List<string>();

                foreach (var row in rows)
                {
                    try
                    {
                        var siteName = row.Cell(1).GetString();
                        var description = row.Cell(2).GetString();
                        var address = row.Cell(3).GetString();
                        var apartment = row.Cell(4).GetString();
                        var countryName = row.Cell(5).GetString();
                        var stateName = row.Cell(6).GetString();
                        var cityName = row.Cell(7).GetString();
                        var zipCode = row.Cell(8).GetString();

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(siteName))
                        {
                            errors.Add($"Row {row.RowNumber()}: Site Name is required");
                            continue;
                        }

                        // Get country ID
                        var country = _locationDataService.GetCountries()
                            .FirstOrDefault(c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase));
                        if (country == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: Country '{countryName}' not found");
                            continue;
                        }

                        // Get state ID
                        var state = _locationDataService.GetStatesByCountry(country.Id)
                            .FirstOrDefault(s => s.Name.Equals(stateName, StringComparison.OrdinalIgnoreCase));
                        if (state == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: State '{stateName}' not found for country '{countryName}'");
                            continue;
                        }

                        // Get city ID
                        var city = _locationDataService.GetCitiesByState(state.Id)
                            .FirstOrDefault(c => c.Name.Equals(cityName, StringComparison.OrdinalIgnoreCase));
                        if (city == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: City '{cityName}' not found for state '{stateName}'");
                            continue;
                        }

                        // Get company ID
                        var companyId = _authService.GetCurrentUserCompanyId();
                        if (companyId == null)
                        {
                            errors.Add($"Row {row.RowNumber()}: User does not have an associated company");
                            continue;
                        }

                        // Create new site
                        var site = new Site
                        {
                            Name = siteName,
                            Description = description,
                            Address = address + (string.IsNullOrWhiteSpace(apartment) ? "" : $", {apartment}"),
                            CountryId = country.Id,
                            StateId = state.Id,
                            CityId = city.Id,
                            ZipCode = zipCode,
                            CompanyId = companyId.Value // Use the non-nullable value
                        };

                        importedSites.Add(site);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row.RowNumber()}: Error processing row - {ex.Message}");
                    }
                }

                // Save valid sites to database
                if (importedSites.Any())
                {
                    foreach (var site in importedSites)
                    {
                        await _siteRepository.AddAsync(site);
                    }
                }

                return Ok(new
                {
                    Success = true,
                    ImportedCount = importedSites.Count,
                    ErrorCount = errors.Count,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing file: {ex.Message}");
            }
        }
    }
} 