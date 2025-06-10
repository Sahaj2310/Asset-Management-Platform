using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace AssetWeb.Models
{
    public class City
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("state_id")]
        public int StateId { get; set; }
    }
} 