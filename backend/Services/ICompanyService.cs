using System;
using System.Threading.Tasks;
using AssetWeb.DTOs;
using AssetWeb.Models;

namespace AssetWeb.Services
{
    public interface ICompanyService
    {
        Task<CompanyResponse?> GetCompanyByUserIdAsync(Guid userId);
        Task<CompanyResponse> CreateCompanyAsync(CompanyRegistrationRequest request, Guid userId);
        Task<CompanyResponse?> UpdateCompanyAsync(CompanyRegistrationRequest request, Guid userId);
        Task<bool> UserHasCompanyAsync(Guid userId);
    }
} 