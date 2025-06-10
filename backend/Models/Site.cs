using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AssetWeb.Models
{
    public class Site
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CompanyId { get; set; } 

        [ForeignKey("CompanyId")]
        public Company? Company { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int CountryId { get; set; }
        [Required]
        public int StateId { get; set; }
        [Required]
        public int CityId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        public ICollection<Location> Locations { get; set; } = new List<Location>();
    }
} 