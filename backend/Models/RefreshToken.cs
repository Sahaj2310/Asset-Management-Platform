using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetWeb.Models
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? ReplacedByToken { get; set; }

        public string? ReasonRevoked { get; set; }
    }
} 