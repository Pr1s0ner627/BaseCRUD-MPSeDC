using BaseCRUD.CookieAuth.Models;

namespace BaseCRUD.CookieAuth.Repos
{
    public interface IUserRepository
    {
        Task<Users?> GetByEmailAsync(string normalizedEmail);
        Task<int> CreateAsync(Users user);
        Task<bool> EmailExistsAsync(string normalizedEmail);
        Task<Users?> GetbyIdAsync(int userId);
    }
}
