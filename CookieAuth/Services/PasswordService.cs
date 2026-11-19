using Microsoft.AspNetCore.Identity;

namespace BaseCRUD.CookieAuth.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<string> _passwordHasher = new();
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword("Authentication", password);
        }

        public bool VerifyPassword(string providedPassword, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword("Authentication", hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
