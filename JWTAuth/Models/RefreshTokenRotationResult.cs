namespace BaseCRUD.JWTAuth.Models
{
    public class RefreshTokenRotationResult
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
