using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;

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
                string createUserAccountTableQuery = @"CREATE TABLE IF NOT EXISTS user(
                                                     Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                     Name TEXT NOT NULL,
                                                     PhoneNumber TEXT NOT NULL UNIQUE,
                                                     Email TEXT NOT NULL UNIQUE,
                                                     HashedPassword TEXT NOT NULL
                                                    )";
                using (var createUserAccountTableQueryCommand = new SqliteCommand(createUserAccountTableQuery, connection, transaction))
                {
                    createUserAccountTableQueryCommand.ExecuteNonQuery();
                }


                string createFRATableQuery = @"CREATE TABLE IF NOT EXISTS fra(
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            FraName TEXT NOT NULL UNIQUE,
                                            date DATETIME NOT NULL
                                            )";
                using (var createFRATableQueryCommand = new SqliteCommand(createFRATableQuery, connection, transaction))
                {
                    createFRATableQueryCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }
    }
}
