using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        [MinLength(8)]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}