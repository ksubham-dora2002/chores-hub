using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class CreateShoppingProductDTO
    {
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsBought { get; set; } = false;
        [Required]
        public Guid UserId { get; set; }
    }
}