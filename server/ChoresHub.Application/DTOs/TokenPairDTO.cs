using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class TokenPairDTO
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        [Required]
        public string RefreshTokenString { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}