using System.Text.Json.Serialization;

namespace AssetWeb.Models
{
    public class State
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("country_id")]
        public int CountryId { get; set; }
    }
} 