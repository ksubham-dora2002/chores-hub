namespace ChoresHub.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
    }
}