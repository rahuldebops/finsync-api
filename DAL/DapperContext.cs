using Npgsql;
using System.Data;

namespace finsyncapi.DAL
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionReportString;
        private readonly string _connectionReplicationDBString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DB1Connection");
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    }
}
