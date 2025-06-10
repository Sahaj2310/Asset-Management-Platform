using AssetWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssetWeb.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class GeographicalLocationsController : ControllerBase
    {
        private readonly LocationDataService _locationService;

        public GeographicalLocationsController(LocationDataService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            var countries = _locationService.GetCountries()
                .Select(c => new { c.Id, c.Name });
            return Ok(countries);
        }

        [HttpGet("states")]
        public IActionResult GetStates(int countryId)
        {
            var states = _locationService.GetStatesByCountry(countryId)
                .Select(s => new { s.Id, s.Name });
            return Ok(states);
        }

        [HttpGet("cities")]
        public IActionResult GetCities(int stateId)
        {
            var cities = _locationService.GetCitiesByState(stateId)
                .Select(c => new { c.Id, c.Name });
            return Ok(cities);
        }
    }
} 