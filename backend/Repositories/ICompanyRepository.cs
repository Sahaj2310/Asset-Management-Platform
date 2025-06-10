using System;
using System.Threading.Tasks;
using AssetWeb.Models;

namespace AssetWeb.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetCompanyByUserIdAsync(Guid userId);
        Task<bool> HasCompanyAsync(Guid userId);
        Task<Company?> GetCompanyByIdAsync(Guid id);
        Task<Company> CreateCompanyAsync(Company company);
        Task<Company?> UpdateCompanyAsync(Company company);
    }
} 