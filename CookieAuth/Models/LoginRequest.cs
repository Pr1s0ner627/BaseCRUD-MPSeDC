using Microsoft.AspNetCore.SignalR;

namespace BaseCRUD.CookieAuth.Models
{
    public class LoginRequest
    {
        public string Email { get; set;} = String.Empty;
        public string Password { get; set; } = String.Empty;

    }
}
