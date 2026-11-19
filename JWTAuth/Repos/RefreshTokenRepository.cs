using Microsoft.Data.SqlClient;
using BaseCRUD.CookieAuth.Data;
using BaseCRUD.JWTAuth.Models;

namespace BaseCRUD.JWTAuth.Repos
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SQLConnectionFactory _connectionFactory;

        public RefreshTokenRepository(SQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateAsync(RefreshTokens refreshToken)
        {
            const string query = @"
                INSERT INTO RefreshTokens (UserId, TokenHash, ExpiresAt)
                VALUES (@UserId, @TokenHash, @ExpiresAt)";

            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", refreshToken.UserId);
            command.Parameters.AddWithValue("@TokenHash", refreshToken.TokenHash);
            command.Parameters.AddWithValue("@ExpiresAt", refreshToken.ExpiresAt);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<RefreshTokens?> GetByTokenHashAsync(byte[] tokenHash)
        {
            const string query = @"
                SELECT TokenId, UserId, TokenHash, CreatedAt, ExpiresAt, RevokedAt
                FROM RefreshTokens
                WHERE TokenHash = @TokenHash";
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@TokenHash", tokenHash);
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RefreshTokens
                {
                    TokenId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TokenHash = (byte[])reader[2],
                    CreatedAt = reader.GetDateTime(3),
                    ExpiresAt = reader.GetDateTime(4),
                    RevokedAt = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5)
                };
            }
            return null;
        }
        
        public async Task RevokeAsync(int tokenId)
        {
            const string query = @"
                UPDATE RefreshTokens
                SET RevokedAt = @RevokedAt
                WHERE TokenId = @TokenId";
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RevokedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@TokenId", tokenId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task RevokeAllForUserAsync(int userId)
        {
            const string query = @"
                UPDATE RefreshTokens
                SET RevokedAt = SYSUTCDATETIME()
                WHERE UserId = @UserId AND RevokedAt IS NULL";
            using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RevokedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@UserId", userId);
            await command.ExecuteNonQueryAsync();
        }
    }
}
