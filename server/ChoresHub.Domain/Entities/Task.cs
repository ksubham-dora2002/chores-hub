namespace ChoresHub.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDone { get; set; } = false;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}