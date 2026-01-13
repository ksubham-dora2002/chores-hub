using System.ComponentModel.DataAnnotations;

namespace ChoresHub.Application.DTOs
{
    public class ShoppingProductDTO
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsBought { get; set; } = false;
        public string UserName { get; set; } = "";
        public string UserPicture { get; set; } = "";
        [Required]
        public Guid UserId { get; set; }
    }
}