using System;
using System.ComponentModel.DataAnnotations;

namespace AssetWeb.DTOs
{
    public class CreateLocationRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid SiteId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }
    }

    public class LocationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
} 