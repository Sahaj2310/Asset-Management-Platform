using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetWeb.Models
{
    public class Site
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CompanyId { get; set; } 

        [ForeignKey("CompanyId")]
        public Company Company { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }

        [MaxLength(20)]
        public string ZipCode { get; set; }
    }
} 