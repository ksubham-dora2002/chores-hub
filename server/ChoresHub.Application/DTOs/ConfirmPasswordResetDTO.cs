using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class ConfirmPasswordResetDTO
    {
        [Required]
        public string Token { get; set; } = "";
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = "";
    }
}
