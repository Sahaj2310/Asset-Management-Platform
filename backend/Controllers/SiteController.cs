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

namespace AssetWeb.Controllers
{
    [ApiController]
    [Route("api/sites")]
    [Authorize]
    public class SiteController : ControllerBase
    {
        private readonly ISiteRepository _siteRepository;

        public SiteController(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SiteReadDto>>> GetSites()
        {
            var sites = await _siteRepository.GetAllAsync();
            var result = sites.Select(site => new SiteReadDto
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
            });
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SiteReadDto>> GetSite(Guid id)
        {
            var site = await _siteRepository.GetByIdAsync(id);
            if (site == null) return NotFound();
            var dto = new SiteReadDto
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
            return Ok(dto);
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
            var site = await _siteRepository.GetByIdAsync(id);
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
            var site = await _siteRepository.GetByIdAsync(id);
            if (site == null) return NotFound();
            await _siteRepository.DeleteAsync(site);
            return NoContent();
        }
    }
} 