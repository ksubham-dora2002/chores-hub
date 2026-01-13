namespace ChoresHub.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }


        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<ShoppingProduct> ShoppingProducts { get; set; } = new List<ShoppingProduct>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}