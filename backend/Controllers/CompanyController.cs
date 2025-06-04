using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AssetWeb.Models;
using AssetWeb.DTOs;
using AssetWeb.Repositories;
using AssetWeb.Services;
using AutoMapper;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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
        private readonly ICompanyService _companyService;
        private readonly IConfiguration _configuration;
        private readonly string _uploadDirectory;

        public CompanyController(
            ICompanyRepository companyRepository, 
            IUserService userService,
            IMapper mapper,
            ICompanyService companyService,
            IConfiguration configuration)
        {
            _companyRepository = companyRepository;
            _userService = userService;
            _mapper = mapper;
            _companyService = companyService;
            _configuration = configuration;
            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "company-logos");
            
            // Create the upload directory if it doesn't exist
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        [HttpGet]
        public async Task<ActionResult<CompanyResponse>> GetCompany()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
                {
                    return Unauthorized(new { message = "Invalid user" });
                }

                var company = await _companyRepository.GetCompanyByUserIdAsync(userGuid);
                if (company == null)
                {
                    return NotFound(new { message = "Company not found" });
                }

                return Ok(_mapper.Map<CompanyResponse>(company));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterCompany([FromForm] CompanyRegistrationRequest request)
        {
            try
            {
                // Handle logo upload if present
                if (Request.Form.Files.Count > 0)
                {
                    var logoFile = Request.Form.Files[0];
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(logoFile.FileName)}";
                        var filePath = Path.Combine(_uploadDirectory, fileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(stream);
                        }

                        // Set the logo path in the request
                        request.LogoPath = $"/uploads/company-logos/{fileName}";
                    }
                }

                // Get the current user's ID from the token
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
                {
                    return Unauthorized(new { message = "Invalid user" });
                }

                var result = await _companyService.CreateCompanyAsync(request, userGuid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCompany([FromForm] CompanyRegistrationRequest request)
        {
            try
            {
                // Handle logo upload if present
                if (Request.Form.Files.Count > 0)
                {
                    var logoFile = Request.Form.Files[0];
                    if (logoFile != null && logoFile.Length > 0)
                    {
                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(logoFile.FileName)}";
                        var filePath = Path.Combine(_uploadDirectory, fileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await logoFile.CopyToAsync(stream);
                        }

                        // Set the logo path in the request
                        request.LogoPath = $"/uploads/company-logos/{fileName}";
                    }
                }

                // Get the current user's ID from the token
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
                {
                    return Unauthorized(new { message = "Invalid user" });
                }

                var result = await _companyService.UpdateCompanyAsync(request, userGuid);
                if (result == null)
                {
                    return NotFound(new { message = "Company not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 