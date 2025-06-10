using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AssetWeb.DTOs;
using AssetWeb.Models;
using AssetWeb.Repositories;

namespace AssetWeb.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationService> _logger;

        public LocationService(
            ILocationRepository locationRepository,
            ISiteRepository siteRepository,
            ICompanyRepository companyRepository,
            IMapper mapper,
            ILogger<LocationService> logger)
        {
            _locationRepository = locationRepository;
            _siteRepository = siteRepository;
            _companyRepository = companyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, LocationResponse? Location)> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetLocationByIdAsync(id);
            if (location == null)
            {
                return (false, "Location not found", null);
            }
            return (true, "Location retrieved successfully", _mapper.Map<LocationResponse>(location));
        }

        public async Task<(bool Success, string Message, IEnumerable<LocationResponse> Locations)> GetAllLocationsAsync()
        {
            var locations = await _locationRepository.GetAllLocationsAsync();
            return (true, "Locations retrieved successfully", _mapper.Map<IEnumerable<LocationResponse>>(locations));
        }

        public async Task<(bool Success, string Message, IEnumerable<LocationResponse> Locations)> GetLocationsBySiteIdAsync(Guid siteId)
        {
            var locations = await _locationRepository.GetLocationsBySiteIdAsync(siteId);
            return (true, "Locations retrieved successfully", _mapper.Map<IEnumerable<LocationResponse>>(locations));
        }

        public async Task<(bool Success, string Message, IEnumerable<LocationResponse> Locations)> GetLocationsByCompanyIdAsync(Guid companyId)
        {
            var locations = await _locationRepository.GetLocationsByCompanyIdAsync(companyId);
            return (true, "Locations retrieved successfully", _mapper.Map<IEnumerable<LocationResponse>>(locations));
        }

        public async Task<(bool Success, string Message, LocationResponse? Location)> CreateLocationAsync(CreateLocationRequest request)
        {
            _logger.LogInformation("Attempting to create location: {LocationName} for SiteId: {SiteId}", request.Name, request.SiteId);

            var site = await _siteRepository.GetSiteByIdAsync(request.SiteId);
            if (site == null)
            {
                _logger.LogWarning("Location creation failed: Site with ID {SiteId} not found.", request.SiteId);
                return (false, "Site not found", null);
            }

            var company = await _companyRepository.GetCompanyByIdAsync(request.CompanyId);
            if (company == null)
            {
                _logger.LogWarning("Location creation failed: Company with ID {CompanyId} not found.", request.CompanyId);
                return (false, "Company not found", null);
            }

            if (site.CompanyId != request.CompanyId)
            {
                _logger.LogWarning("Location creation failed: Site {SiteId} does not belong to Company {CompanyId}.", request.SiteId, request.CompanyId);
                return (false, "Site does not belong to the specified company", null);
            }

            if (await _locationRepository.LocationExistsAsync(request.Name, request.SiteId))
            {
                _logger.LogWarning("Location creation failed: Location '{LocationName}' already exists for Site ID {SiteId}.", request.Name, request.SiteId);
                return (false, "Location with this name already exists for the specified site", null);
            }

            var location = _mapper.Map<Location>(request);
            var createdLocation = await _locationRepository.CreateLocationAsync(location);
            _logger.LogInformation("Location created successfully: {LocationName}", createdLocation.Name);
            return (true, "Location created successfully", _mapper.Map<LocationResponse>(createdLocation));
        }

        public async Task<(bool Success, string Message)> UpdateLocationAsync(Guid id, CreateLocationRequest request)
        {
            _logger.LogInformation("Attempting to update location with ID: {LocationId}", id);

            var existingLocation = await _locationRepository.GetLocationByIdAsync(id);
            if (existingLocation == null)
            {
                _logger.LogWarning("Location update failed: Location with ID {LocationId} not found.", id);
                return (false, "Location not found");
            }
            
            if (existingLocation.Name != request.Name && await _locationRepository.LocationExistsAsync(request.Name, request.SiteId))
            {
                _logger.LogWarning("Location update failed: Another location with name '{LocationName}' already exists for Site ID {SiteId}.", request.Name, request.SiteId);
                return (false, "Another location with this name already exists for the specified site");
            }

            _mapper.Map(request, existingLocation);
            var success = await _locationRepository.UpdateLocationAsync(existingLocation);
            if (success)
            {
                _logger.LogInformation("Location updated successfully: {LocationName}", existingLocation.Name);
                return (true, "Location updated successfully");
            }
            else
            {
                _logger.LogError("Location update failed for ID: {LocationId}", id);
                return (false, "Failed to update location");
            }
        }

        public async Task<(bool Success, string Message)> DeleteLocationAsync(Guid id)
        {
            _logger.LogInformation("Attempting to delete location with ID: {LocationId}", id);
            var success = await _locationRepository.DeleteLocationAsync(id);
            if (success)
            {
                _logger.LogInformation("Location deleted successfully with ID: {LocationId}", id);
                return (true, "Location deleted successfully");
            }
            else
            {
                _logger.LogWarning("Location deletion failed: Location with ID {LocationId} not found.", id);
                return (false, "Location not found");
            }
        }
    }
} 