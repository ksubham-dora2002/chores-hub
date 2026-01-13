using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class DeleteUserDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}