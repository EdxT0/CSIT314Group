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

                using var pragma = new SqliteCommand("PRAGMA foreign_keys = ON;", connection, transaction);
                pragma.ExecuteNonQuery();

                //string dropUserProfileTableQuery = @"DROP TABLE UserProfile";
                //using (var dropUserProfileTableQueryCommand = new SqliteCommand(dropUserProfileTableQuery, connection, transaction))
                //{
                //    dropUserProfileTableQueryCommand.ExecuteNonQuery();
                //}

                string createUserProfileTableQuery = @"CREATE TABLE IF NOT EXISTS UserProfile(
                                            Id INTEGER PRIMARY KEY,
                                            ProfileName TEXT NOT NULL UNIQUE,
                                            Description TEXT,
                                            Status BOOL NOT NULL
                                            )";
                using (var createUserProfileTableQueryCommand = new SqliteCommand(createUserProfileTableQuery, connection, transaction))
                {
                    createUserProfileTableQueryCommand.ExecuteNonQuery();
                }

                string createUserAccountTableQuery = @"CREATE TABLE IF NOT EXISTS UserAccount(
                                                     Id INTEGER PRIMARY KEY,
                                                     Name TEXT NOT NULL,
                                                     PhoneNumber TEXT NOT NULL UNIQUE,
                                                     Email TEXT NOT NULL UNIQUE,
                                                     IsSuspended BOOL NOT NULL,
                                                     HashedPassword TEXT NOT NULL,
                                                     ProfileId INTEGER NOT NULL,
                                                     FOREIGN KEY (ProfileId) References UserProfile(Id)
                                                    )";
                using (var createUserAccountTableQueryCommand = new SqliteCommand(createUserAccountTableQuery, connection, transaction))
                {
                    createUserAccountTableQueryCommand.ExecuteNonQuery();
                }

                //resets the tables below
                //string dropFraTableQuery = @"DROP TABLE FundraiserActivity";
                //using (var dropFraTableQueryCommand = new SqliteCommand(dropFraTableQuery, connection, transaction))
                //{
                //    dropFraTableQueryCommand.ExecuteNonQuery();
                //}
                //string dropUserFundraiserTableQuery = @"DROP TABLE UserFundraiser";
                //using (var dropUserFundraiserTableQueryCommand = new SqliteCommand(dropUserFundraiserTableQuery, connection, transaction))
                //{
                //    dropUserFundraiserTableQueryCommand.ExecuteNonQuery();
                //}
                //string dropFavouriteListTableQuery = @"DROP TABLE FavouriteList";
                //using (var dropFavouriteListTableQueryCommand = new SqliteCommand(dropFavouriteListTableQuery, connection, transaction))
                //{
                //    dropFavouriteListTableQueryCommand.ExecuteNonQuery();
                //}
                //string dropFundraiserDonationsTableQuery = @"DROP TABLE FundraiserDonations";
                //using (var dropFundraiserDonationsTableQueryCommand = new SqliteCommand(dropFundraiserDonationsTableQuery, connection, transaction))
                //{
                //    dropFundraiserDonationsTableQueryCommand.ExecuteNonQuery();
                //}

                string createFRATableQuery = @"CREATE TABLE IF NOT EXISTS FundraiserActivity(
                                            Id INTEGER PRIMARY KEY,
                                            FraName TEXT NOT NULL UNIQUE,
                                            Description TEXT,
                                            Deadline TEXT NOT NULL,
                                            Status BOOL NOT NULL,
                                            AmtOfViews Integer,
                                            AmtDonated REAL,    
                                            AmtRequested REAL
                                            )";


                using (var createFRATableQueryCommand = new SqliteCommand(createFRATableQuery, connection, transaction))
                {
                    createFRATableQueryCommand.ExecuteNonQuery();
                }

                string createUserFundraiserTableQuery = @"CREATE TABLE IF NOT EXISTS UserFundraiser(
                                            Id INTEGER PRIMARY KEY,
                                            UserId INTEGER NOT NULL,
                                            FraId INTEGER NOT NULL,
                                            UNIQUE (UserId, FraId),
                                            FOREIGN KEY (UserId) REFERENCES UserAccount(Id),
                                            FOREIGN KEY (FraId) REFERENCES FundraiserActivity(Id) ON DELETE CASCADE
                                            )";
                using (var createUserFundraiserTableQueryCommand = new SqliteCommand(createUserFundraiserTableQuery, connection, transaction))
                {
                    createUserFundraiserTableQueryCommand.ExecuteNonQuery();
                }

                string createFavouriteListTableQuery = @"CREATE TABLE IF NOT EXISTS FavouriteList(
                                            Id INTEGER PRIMARY KEY,
                                            UserId INTEGER NOT NULL,
                                            FraId INTEGER NOT NULL,
                                            UNIQUE (UserId, FraId),
                                            FOREIGN KEY (UserId) REFERENCES UserAccount(Id),
                                            FOREIGN KEY (FraId) REFERENCES FundraiserActivity(Id)
                                            )";
                using (var createFavouriteListTableQueryCommand = new SqliteCommand(createFavouriteListTableQuery, connection, transaction))
                {
                    createFavouriteListTableQueryCommand.ExecuteNonQuery();
                }

                string createFundraiserDonationsTableQuery = @"CREATE TABLE IF NOT EXISTS FundraiserDonations(
                                            Id INTEGER PRIMARY KEY,
                                            UserId INTEGER NOT NULL,
                                            FraId INTEGER NOT NULL,
                                            AmtDonatedByUser REAL,
                                            DateDonated TEXT,
                                            FOREIGN KEY (UserId) REFERENCES UserAccount(Id),
                                            FOREIGN KEY (FraId) REFERENCES FundraiserActivity(Id)
                                            )";
                using (var createFundraiserDonationsTableQueryCommand = new SqliteCommand(createFundraiserDonationsTableQuery, connection, transaction))
                {
                    createFundraiserDonationsTableQueryCommand.ExecuteNonQuery();
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
            string checkIfTableHasAnyValue = @"select id from UserProfile where id = 1";
            using var checkIfTableHasAnyValueCommand = new SqliteCommand(checkIfTableHasAnyValue, connection, transaction);
            object? result = checkIfTableHasAnyValueCommand.ExecuteScalar();
            if(result == null)
            {
                string seedProfileTableQuery = @"INSERT INTO UserProfile ( ProfileName, Description, Status ) VALUES (@Name, @Desc, @status)";

                string[] UserProfileNameList = { "admin","platform manager", "donee", "fundraiser manager" };
                string[] UserProfileDescList = { "To manage account", "To manage fundraiser categories", "To contribute to fundraisers", "To manage fundraisers" };


                using (var seedProfileTableQueryCommand = new SqliteCommand(seedProfileTableQuery, connection, transaction))
                {
                    seedProfileTableQueryCommand.Parameters.Add("@Name", SqliteType.Text);
                    seedProfileTableQueryCommand.Parameters.Add("@Desc", SqliteType.Text);
                    seedProfileTableQueryCommand.Parameters.Add("@status", SqliteType.Integer);

                    for (int i = 0; i < UserProfileNameList.Length; i++) 
                    {
                        seedProfileTableQueryCommand.Parameters["@Name"].Value = UserProfileNameList[i];
                        seedProfileTableQueryCommand.Parameters["@Desc"].Value = UserProfileDescList[i];
                        seedProfileTableQueryCommand.Parameters["@status"].Value = 1;
                        seedProfileTableQueryCommand.ExecuteNonQuery();
                    }
                }
            }
            
        }
    }
}
