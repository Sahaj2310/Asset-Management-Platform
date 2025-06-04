using AssetWeb.Models;
using System.Text.Json;

namespace AssetWeb.Services
{
    public class LocationDataService
    {
        private readonly List<Country> _countries;
        private readonly List<State> _states;
        private readonly List<City> _cities;

        public LocationDataService(IWebHostEnvironment env)
        {
            var dataPath = Path.Combine(env.WebRootPath, "data");
            _countries = JsonSerializer.Deserialize<List<Country>>(File.ReadAllText(Path.Combine(dataPath, "countries.json")));
            _states = JsonSerializer.Deserialize<List<State>>(File.ReadAllText(Path.Combine(dataPath, "states.json")));
            _cities = JsonSerializer.Deserialize<List<City>>(File.ReadAllText(Path.Combine(dataPath, "cities.json")));
        }

        public IEnumerable<Country> GetCountries() => _countries;
        public IEnumerable<State> GetStatesByCountry(int countryId) => _states.Where(s => s.CountryId == countryId);
        public IEnumerable<City> GetCitiesByState(int stateId) => _cities.Where(c => c.StateId == stateId);
    }
} 