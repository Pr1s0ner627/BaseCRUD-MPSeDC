using BaseCRUD.JWTAuth.Models;
using BaseCRUD.JWTAuth.Repos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace BaseCRUD.JWTAuth.Services
{
    public class RefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
            string refreshToken = WebEncoders.Base64UrlEncode(randomBytes);
            byte[] tokenHash = SHA256.HashData(randomBytes);

            var refreshTokenRecord = new RefreshTokens
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            await _refreshTokenRepository.CreateAsync(refreshTokenRecord);
            return refreshToken;
        }

        public async Task<RefreshTokens?> FindAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            byte[] bytes;
            try { bytes = WebEncoders.Base64UrlDecode(token); }
            catch (FormatException) { return null; }
            if (bytes.Length != 32) return null;

            return await _refreshTokenRepository.GetByTokenHashAsync(SHA256.HashData(bytes));
        }

        public async Task<RefreshTokens?> ValidateRefreshTokenAsync(string token)
        {
            var storedToken = await FindAsync(token);
            if(storedToken == null || storedToken.ExpiresAt <= DateTime.UtcNow || storedToken.UserId != null)
                return null;

            return storedToken;
        }

        public async Task<RefreshTokenRotationResult?> RotateRefreshTokenAsync(string token)
        {
            var storeToken = await FindAsync(token);

            if (storeToken == null)
                return null;

            await _refreshTokenRepository.RevokeAsync(storeToken.UserId);
            var newRefreshToken = await GenerateRefreshTokenAsync(storeToken.UserId);

            return new RefreshTokenRotationResult
            {
                UserId = storeToken.UserId,
                RefreshToken = newRefreshToken
            };
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var storedToken = await FindAsync(token);

            if(storedToken != null)
                await _refreshTokenRepository.RevokeAsync(storedToken.TokenId);
        }

        public async Task RevokeAllRefreshTokenAsync(int userId)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(userId);
        }
    }
}
