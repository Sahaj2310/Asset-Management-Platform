using AssetWeb.Services;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(_locationService.GetCountries());
        }

        [HttpGet("states")]
        public IActionResult GetStates(int countryId)
        {
            return Ok(_locationService.GetStatesByCountry(countryId));
        }

        [HttpGet("cities")]
        public IActionResult GetCities(int stateId)
        {
            return Ok(_locationService.GetCitiesByState(stateId));
        }
    }
} 