using Microsoft.Data.SqlClient;

namespace BaseCRUD.CookieAuth.Data
{
    public sealed class SQLConnectionFactory
    {
        private readonly string _connectionString;
        public SQLConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not found.");
        }
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
