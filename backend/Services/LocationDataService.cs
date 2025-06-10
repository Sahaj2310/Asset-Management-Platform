using AssetWeb.Models;
using System.Text.Json;

namespace AssetWeb.Services
{
    public class LocationDataService
    {
        private readonly List<Country>? _countries;
        private readonly List<State>? _states;
        private readonly List<City>? _cities;

        public LocationDataService(IWebHostEnvironment env)
        {
            var dataPath = Path.Combine(env.WebRootPath, "data");
            _countries = JsonSerializer.Deserialize<List<Country>>(File.ReadAllText(Path.Combine(dataPath, "countries.json"))) ?? new List<Country>();
            _states = JsonSerializer.Deserialize<List<State>>(File.ReadAllText(Path.Combine(dataPath, "states.json"))) ?? new List<State>();
            _cities = JsonSerializer.Deserialize<List<City>>(File.ReadAllText(Path.Combine(dataPath, "cities.json"))) ?? new List<City>();
        }

        public IEnumerable<Country> GetCountries() => _countries ?? new List<Country>();
        public IEnumerable<State> GetStatesByCountry(int countryId) => (_states ?? new List<State>()).Where(s => s.CountryId == countryId);
        public IEnumerable<City> GetCitiesByState(int stateId) => (_cities ?? new List<City>()).Where(c => c.StateId == stateId);
    }
} 