using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace AssetWeb.Models
{
    public class Country
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("iso3")]
        [Required]
        [StringLength(3)]
        public string Iso3 { get; set; } = string.Empty;
        [JsonPropertyName("iso2")]
        [Required]
        [StringLength(2)]
        public string Iso2 { get; set; } = string.Empty;
    }
} 