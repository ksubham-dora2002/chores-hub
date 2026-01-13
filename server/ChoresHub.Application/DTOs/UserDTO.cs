using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}