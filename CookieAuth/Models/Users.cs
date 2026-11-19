namespace BaseCRUD.CookieAuth.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string Email { get; set; } = String.Empty;
        public string NormalizedEmail { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public string DisplayName { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

    }
}