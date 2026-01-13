using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsSeen { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
        public string UserPicture { get; set; } = string.Empty;
        [Required]
        public Guid UserId { get; set; }
    }
}