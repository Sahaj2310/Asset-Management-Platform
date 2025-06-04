using System;
using System.Threading.Tasks;
using AssetWeb.DTOs;
using AssetWeb.Models;
using AssetWeb.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AssetWeb.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(
            ICompanyRepository companyRepository,
            IMapper mapper,
            ILogger<CompanyService> logger)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CompanyResponse?> GetCompanyByUserIdAsync(Guid userId)
        {
            var company = await _companyRepository.GetCompanyByUserIdAsync(userId);
            return company != null ? _mapper.Map<CompanyResponse>(company) : null;
        }

        public async Task<CompanyResponse> CreateCompanyAsync(CompanyRegistrationRequest request, Guid userId)
        {
            var hasCompany = await _companyRepository.HasCompanyAsync(userId);
            if (hasCompany)
            {
                throw new InvalidOperationException("User already has a company");
            }

            var company = _mapper.Map<Company>(request);
            company.UserId = userId;

            var createdCompany = await _companyRepository.CreateCompanyAsync(company);
            return _mapper.Map<CompanyResponse>(createdCompany);
        }

        public async Task<CompanyResponse?> UpdateCompanyAsync(CompanyRegistrationRequest request, Guid userId)
        {
            var existingCompany = await _companyRepository.GetCompanyByUserIdAsync(userId);
            if (existingCompany == null)
            {
                return null;
            }

            _mapper.Map(request, existingCompany);
            var updatedCompany = await _companyRepository.UpdateCompanyAsync(existingCompany);
            return updatedCompany != null ? _mapper.Map<CompanyResponse>(updatedCompany) : null;
        }

        public async Task<bool> UserHasCompanyAsync(Guid userId)
        {
            return await _companyRepository.HasCompanyAsync(userId);
        }
    }
} 