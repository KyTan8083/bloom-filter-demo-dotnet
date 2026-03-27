using Microsoft.Data.SqlClient;
using System.Data;

namespace BloomFilterDemo.Services
{
    public sealed class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MetricsDb")
                                ?? throw new InvalidOperationException("Connection string 'MetricsDb' is missing.");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
