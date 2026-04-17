using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Transactions;

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
                                                     Profile TEXT NOT NULL,
                                                     PhoneNumber TEXT NOT NULL UNIQUE,
                                                     Email TEXT NOT NULL UNIQUE,
                                                     IsSuspended BOOL NOT NULL,
                                                     HashedPassword TEXT NOT NULL
                                                    )";
                using (var createUserAccountTableQueryCommand = new SqliteCommand(createUserAccountTableQuery, connection, transaction))
                {
                    createUserAccountTableQueryCommand.ExecuteNonQuery();
                }


                string createUserProfileTableQuery = @"CREATE TABLE IF NOT EXISTS userprofile(
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            ProfileName TEXT NOT NULL UNIQUE
                                            
                                            )";
                using (var createUserProfileTableQueryCommand = new SqliteCommand(createUserProfileTableQuery, connection, transaction))
                {
                    createUserProfileTableQueryCommand.ExecuteNonQuery();
                }


                string createFRATableQuery = @"CREATE TABLE IF NOT EXISTS fra(
                                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                            FraName TEXT NOT NULL UNIQUE,
                                            Description TEXT,
                                            date DATETIME NOT NULL
                                            )";


                using (var createFRATableQueryCommand = new SqliteCommand(createFRATableQuery, connection, transaction))
                {
                    createFRATableQueryCommand.ExecuteNonQuery();
                }

                seedProfile(connection, transaction);
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }

        public static void seedProfile(SqliteConnection connection, SqliteTransaction transaction)
        {
            string checkIfTableHasAnyValue = @"select id from userprofile where id = 1";
            using var checkIfTableHasAnyValueCommand = new SqliteCommand(checkIfTableHasAnyValue, connection, transaction);
            object? result = checkIfTableHasAnyValueCommand.ExecuteScalar();
            if(result == null)
            {
                string seedProfileTableQuery = @"INSERT INTO userProfile ( ProfileName ) VALUES (@Name)";
                string[] profileExampleList = { "admin", "user", "Fundraiser Manager" };

                using (var seedProfileTableQueryCommand = new SqliteCommand(seedProfileTableQuery, connection, transaction))
                {
                    seedProfileTableQueryCommand.Parameters.Add("@Name", SqliteType.Text);

                    foreach (var profile in profileExampleList)
                    {
                        seedProfileTableQueryCommand.Parameters["@Name"].Value = profile;
                        seedProfileTableQueryCommand.ExecuteNonQuery();
                    }
                }
            }
            
        }
    }
}
