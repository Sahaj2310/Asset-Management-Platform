using System.Text.Json.Serialization;

namespace AssetWeb.Models
{
    public class City
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("state_id")]
        public int StateId { get; set; }
    }
} 