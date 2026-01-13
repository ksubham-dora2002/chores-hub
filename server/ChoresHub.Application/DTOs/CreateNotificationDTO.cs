using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class CreateNotificationDTO
    {
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsSeen { get; set; } = false;
        [Required]
        public Guid UserId { get; set; }
    }
}