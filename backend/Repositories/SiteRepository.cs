using AssetWeb.Data;
using AssetWeb.Models;
using AssetWeb.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetWeb.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        private readonly ApplicationDbContext _context;
        public SiteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Site>> GetAllAsync()
        {
            return await _context.Sites.Include(s => s.Company).ToListAsync();
        }

        public async Task<PaginatedResponse<Site>> GetPaginatedAsync(PaginationParameters parameters)
        {
            var query = _context.Sites.Include(s => s.Company).AsQueryable();
            
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);
            
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return new PaginatedResponse<Site>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<Site?> GetSiteByIdAsync(Guid id)
        {
            return await _context.Sites.Include(s => s.Company).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Site> AddAsync(Site site)
        {
            _context.Sites.Add(site);
            await _context.SaveChangesAsync();
            return site;
        }

        public async Task UpdateAsync(Site site)
        {
            _context.Sites.Update(site);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Site site)
        {
            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();
        }
    }
} 