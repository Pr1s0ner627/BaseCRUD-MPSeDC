namespace BaseCRUD.JWTAuth.Models
{
    public class RefreshTokens
    {
        public int TokenId { get; set; }
        public int UserId { get; set; }
        public byte[] TokenHash { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RevokedAt { get; set; }
    }
}

