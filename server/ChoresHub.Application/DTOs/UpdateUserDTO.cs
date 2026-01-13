using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class UpdateUserDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;

    }
}