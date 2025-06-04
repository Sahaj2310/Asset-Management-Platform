using System.ComponentModel.DataAnnotations;

namespace AssetWeb.DTOs
{
    public class CompanyDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
} 