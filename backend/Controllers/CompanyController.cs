using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AssetWeb.Models;
using AssetWeb.DTOs;
using AssetWeb.Repositories;
using AssetWeb.Services;
using AutoMapper;

namespace AssetWeb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CompanyController(
            ICompanyRepository companyRepository, 
            IUserService userService,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CompanyResponse>> GetCompany()
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var company = await _companyRepository.GetCompanyByUserIdAsync(userId.Value);
            if (company == null)
            {
                return NotFound("Company not found");
            }

            return Ok(_mapper.Map<CompanyResponse>(company));
        }

        [HttpPost]
        public async Task<ActionResult<CompanyResponse>> CreateCompany([FromBody] CompanyRegistrationRequest request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var hasCompany = await _companyRepository.HasCompanyAsync(userId.Value);
            if (hasCompany)
            {
                return BadRequest("User already has a company");
            }

            var company = _mapper.Map<Company>(request);
            company.UserId = userId.Value;

            var createdCompany = await _companyRepository.CreateCompanyAsync(company);
            return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.Id }, _mapper.Map<CompanyResponse>(createdCompany));
        }

        [HttpPut]
        public async Task<ActionResult<CompanyResponse>> UpdateCompany([FromBody] CompanyRegistrationRequest request)
        {
            var userId = _userService.GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var existingCompany = await _companyRepository.GetCompanyByUserIdAsync(userId.Value);
            if (existingCompany == null)
            {
                return NotFound("Company not found");
            }

            _mapper.Map(request, existingCompany);

            var updatedCompany = await _companyRepository.UpdateCompanyAsync(existingCompany);
            if (updatedCompany == null)
            {
                return BadRequest("Failed to update company");
            }

            return Ok(_mapper.Map<CompanyResponse>(updatedCompany));
        }
    }
} 