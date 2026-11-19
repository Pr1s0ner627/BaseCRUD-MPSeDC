namespace BaseCRUD.CookieAuth.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public string DisplayName { get; set; } = String.Empty;
    }
}
