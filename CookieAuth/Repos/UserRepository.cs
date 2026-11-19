using BaseCRUD.CookieAuth.Models;
using BaseCRUD.CookieAuth.Data;
using Microsoft.Data.SqlClient;

namespace BaseCRUD.CookieAuth.Repos
{
    public class UserRepository : IUserRepository
    {
        private readonly SQLConnectionFactory _connectionFactory;

        public UserRepository(SQLConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(Users user)
        {
            const string query = @"
                INSERT INTO Users (Email, NormalizedEmail, PasswordHash, DisplayName, IsActive)
                OUTPUT INSERTED.UserId
                VALUES (@Email, @NormalizedEmail, @PasswordHash, @DisplayName, @IsActive);";

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@NormalizedEmail", user.NormalizedEmail);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);

            object? result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> EmailExistsAsync(string normalizedEmail)
        {
            const string query = @"
                SELECT COUNT(1)
                FROM Users
                WHERE NormalizedEmail = @NormalizedEmail;";

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<Users?> GetByEmailAsync(string normalizedEmail)
        {
            const string query = @"
                SELECT UserId, Email, NormalizedEmail, PasswordHash, DisplayName, CreatedAt, IsActive
                FROM Users
                WHERE NormalizedEmail = @NormalizedEmail";

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            
            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NormalizedEmail", normalizedEmail);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;
            
                return new Users
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    NormalizedEmail = reader.GetString(reader.GetOrdinal("NormalizedEmail")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };
        }
    
        public async Task<Users?> GetbyIdAsync(int userId)
        {
            const string query = @"
                SELECT UserId, Email, NormalizedEmail, PasswordHash, DisplayName, CreatedAt, IsActive
                FROM Users
                WHERE UserId = @UserId";
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new Users
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                NormalizedEmail = reader.GetString(reader.GetOrdinal("NormalizedEmail")),
                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }

    }
}
