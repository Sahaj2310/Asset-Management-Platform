using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AssetWeb.DTOs;

namespace AssetWeb.Services
{
    public interface ILocationService
    {
        Task<(bool Success, string Message, LocationResponse? Location)> GetLocationByIdAsync(Guid id);
        Task<(bool Success, string Message, IEnumerable<LocationResponse> Locations)> GetAllLocationsAsync();
        Task<(bool Success, string Message, IEnumerable<LocationResponse> Locations)> GetLocationsBySiteIdAsync(Guid siteId);
        Task<(bool Success, string Message, IEnumerable<LocationResponse> Locations)> GetLocationsByCompanyIdAsync(Guid companyId);
        Task<(bool Success, string Message, LocationResponse? Location)> CreateLocationAsync(CreateLocationRequest request);
        Task<(bool Success, string Message)> UpdateLocationAsync(Guid id, CreateLocationRequest request);
        Task<(bool Success, string Message)> DeleteLocationAsync(Guid id);
    }
} 