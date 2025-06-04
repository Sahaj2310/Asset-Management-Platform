using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetWeb.Data;
using AssetWeb.Models;
using AssetWeb.DTOs;

namespace AssetWeb.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetCompanyByUserIdAsync(Guid userId)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> HasCompanyAsync(Guid userId)
        {
            return await _context.Companies
                .AnyAsync(c => c.UserId == userId);
        }

        public async Task<Company> CreateCompanyAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company?> UpdateCompanyAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
            return company;
        }
    }
} 