using Microsoft.Data.Sqlite;

namespace CSIT_314_Group.Data
{
    public class DbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqliteConnection CreateConnection()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqliteConnection(connectionString);
        }

    }
}
