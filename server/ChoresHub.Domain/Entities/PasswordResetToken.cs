using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChoresHub.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        public DateTime? UsedAt { get; set; }

        [Required]
        public Guid UserId { get; set; } 

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}
