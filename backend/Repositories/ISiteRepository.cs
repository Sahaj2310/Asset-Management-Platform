using AssetWeb.Models;
using AssetWeb.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetWeb.Repositories
{
    public interface ISiteRepository
    {
        Task<IEnumerable<Site>> GetAllAsync();
        Task<PaginatedResponse<Site>> GetPaginatedAsync(PaginationParameters parameters);
        Task<Site?> GetSiteByIdAsync(Guid id);
        Task<Site> AddAsync(Site site);
        Task UpdateAsync(Site site);
        Task DeleteAsync(Site site);
    }
} 