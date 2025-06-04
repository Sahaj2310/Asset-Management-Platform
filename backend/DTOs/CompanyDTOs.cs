using System;
using System.ComponentModel.DataAnnotations;

namespace AssetWeb.DTOs
{
    public class CompanyRegistrationRequest
    {
        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public int FinancialYearMonth { get; set; }

        [Required]
        public int FinancialYearDay { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; } = string.Empty;

        public string? LogoPath { get; set; }
    }

    public class CompanyResponse
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int FinancialYearMonth { get; set; }
        public int FinancialYearDay { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? LogoPath { get; set; }
    }
} 