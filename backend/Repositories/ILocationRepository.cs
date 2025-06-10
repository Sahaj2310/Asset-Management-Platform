using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetWeb.Models;

namespace AssetWeb.Repositories
{
    public interface ILocationRepository
    {
        Task<Location?> GetLocationByIdAsync(Guid id);
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<IEnumerable<Location>> GetLocationsBySiteIdAsync(Guid siteId);
        Task<IEnumerable<Location>> GetLocationsByCompanyIdAsync(Guid companyId);
        Task<Location> CreateLocationAsync(Location location);
        Task<bool> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(Guid id);
        Task<bool> LocationExistsAsync(string name, Guid siteId);
    }
} 