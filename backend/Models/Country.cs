using System.Text.Json.Serialization;

namespace AssetWeb.Models
{
    public class Country
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("iso3")]
        public string Iso3 { get; set; }
        [JsonPropertyName("iso2")]
        public string Iso2 { get; set; }
    }
} 