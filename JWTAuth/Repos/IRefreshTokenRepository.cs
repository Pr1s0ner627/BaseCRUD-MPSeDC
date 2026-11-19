using BaseCRUD.JWTAuth.Models;

namespace BaseCRUD.JWTAuth.Repos
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshTokens refreshToken);
        Task<RefreshTokens?> GetByTokenHashAsync(byte[] tokenHash);
        Task RevokeAsync(int userId);
        Task RevokeAllForUserAsync(int userId);
    }
}
