using AssetWeb.Services;
using Microsoft.AspNetCore.Mvc;
using AssetWeb.DTOs;

namespace AssetWeb.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly LocationDataService _locationService;

        public LocationsController(LocationDataService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var countries = _locationService.GetCountries()
                .Select(c => new LocationDto { Id = c.Id, Name = c.Name });
            return Ok(countries);
        }

        [HttpGet("states")]
        public IActionResult GetStates(int countryId)
        {
            var states = _locationService.GetStatesByCountry(countryId)
                .Select(s => new LocationDto { Id = s.Id, Name = s.Name });
            return Ok(states);
        }

        [HttpGet("cities")]
        public IActionResult GetCities(int stateId)
        {
            var cities = _locationService.GetCitiesByState(stateId)
                .Select(c => new LocationDto { Id = c.Id, Name = c.Name });
            return Ok(cities);
        }
    }
} 