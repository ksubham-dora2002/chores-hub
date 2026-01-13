using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class RefreshTokenDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string RefreshTokenString { get; set; } = string.Empty;
    }
}