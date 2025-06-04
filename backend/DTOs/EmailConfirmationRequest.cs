using System.ComponentModel.DataAnnotations;

namespace AssetWeb.DTOs
{
    public class EmailConfirmationRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;
    }
} 