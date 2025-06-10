using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetWeb.Models
{
    public class Location
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public Guid SiteId { get; set; }
        [ForeignKey("SiteId")]
        public Site Site { get; set; } = null!;

        public Guid CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; } = null!;
    }
} 