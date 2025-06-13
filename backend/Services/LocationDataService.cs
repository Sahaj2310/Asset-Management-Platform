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
            Console.WriteLine($"Loading location data from: {dataPath}");

            try
            {
                var countriesJson = File.ReadAllText(Path.Combine(dataPath, "countries.json"));
                var statesJson = File.ReadAllText(Path.Combine(dataPath, "states.json"));
                var citiesJson = File.ReadAllText(Path.Combine(dataPath, "cities.json"));

                _countries = JsonSerializer.Deserialize<List<Country>>(countriesJson) ?? new List<Country>();
                _states = JsonSerializer.Deserialize<List<State>>(statesJson) ?? new List<State>();
                _cities = JsonSerializer.Deserialize<List<City>>(citiesJson) ?? new List<City>();

                Console.WriteLine($"Loaded {_countries?.Count ?? 0} countries");
                Console.WriteLine($"Loaded {_states?.Count ?? 0} states");
                Console.WriteLine($"Loaded {_cities?.Count ?? 0} cities");

                // Debug: Print some sample data
                if (_countries?.Any() == true)
                {
                    var india = _countries.FirstOrDefault(c => c.Name == "India");
                    if (india != null)
                    {
                        Console.WriteLine($"India ID: {india.Id}");
                        var indianStates = _states?.Where(s => s.CountryId == india.Id).ToList();
                        Console.WriteLine($"Indian states count: {indianStates?.Count ?? 0}");
                        if (indianStates?.Any() == true)
                        {
                            Console.WriteLine("Sample Indian states:");
                            foreach (var state in indianStates.Take(5))
                            {
                                Console.WriteLine($"State: {state.Name}, ID: {state.Id}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading location data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public IEnumerable<Country> GetCountries() => _countries ?? new List<Country>();
        public IEnumerable<State> GetStatesByCountry(int countryId) => (_states ?? new List<State>()).Where(s => s.CountryId == countryId);
        public IEnumerable<City> GetCitiesByState(int stateId) => (_cities ?? new List<City>()).Where(c => c.StateId == stateId);
    }
} 