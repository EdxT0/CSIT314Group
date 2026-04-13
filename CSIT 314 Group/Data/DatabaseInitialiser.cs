using Microsoft.Data.Sqlite;

namespace CSIT_314_Group.Data
{
    public class DatabaseInitialiser
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public DatabaseInitialiser(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public void InitialiseDatabase()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();

            try
            {
                string createUserAccountTableQuery = @"CREATE TABLE IF NOT EXIST user(
                                                     Id INTEGER PRIMARY KEY AUTO INCREMENT,
                                                     Name TEXT,
                                                     PhoneNumber INTEGER,
                                                     Email TEXT
                                                    )";
                using (var createUserAccountTableQueryCommand = new SqliteCommand(createUserAccountTableQuery, connection, transaction))
                {
                    createUserAccountTableQueryCommand.ExecuteNonQueryAsync();
                }


                string createFRATableQuery = @"CREATE TABLE IF NOT EXIST fra
                                            FraName TEXT,
                                            date DATETIME
                                            ";
                using (var createFRATableQueryCommand = new SqliteCommand(createFRATableQuery, connection, transaction))
                {
                    createFRATableQueryCommand.ExecuteNonQueryAsync();
                }
                transaction.CommitAsync();
            }
            catch
            {
                transaction.RollbackAsync();
                throw;
            }
        
        }
    }
}
