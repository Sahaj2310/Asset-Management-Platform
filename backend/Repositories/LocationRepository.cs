using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetWeb.Data;
using AssetWeb.Models;

namespace AssetWeb.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Location?> GetLocationByIdAsync(Guid id)
        {
            return await _context.Locations
                .Include(l => l.Site)
                .Include(l => l.Company)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            return await _context.Locations
                .Include(l => l.Site)
                .Include(l => l.Company)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocationsBySiteIdAsync(Guid siteId)
        {
            return await _context.Locations
                .Include(l => l.Site)
                .Include(l => l.Company)
                .Where(l => l.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Location>> GetLocationsByCompanyIdAsync(Guid companyId)
        {
            return await _context.Locations
                .Include(l => l.Site)
                .Include(l => l.Company)
                .Where(l => l.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Location> CreateLocationAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<bool> UpdateLocationAsync(Location location)
        {
            _context.Locations.Update(location);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteLocationAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return false;
            }

            _context.Locations.Remove(location);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LocationExistsAsync(string name, Guid siteId)
        {
            return await _context.Locations.AnyAsync(l => l.Name == name && l.SiteId == siteId);
        }
    }
} 