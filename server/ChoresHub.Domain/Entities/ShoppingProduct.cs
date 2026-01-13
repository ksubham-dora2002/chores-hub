namespace ChoresHub.Domain.Entities
{
    public class ShoppingProduct : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsBought { get; set; } = false;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}