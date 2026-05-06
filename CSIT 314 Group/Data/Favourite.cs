using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CSIT_314_Group.Data
{
    public class Favourite
    {

        private readonly DbConnectionFactory _dbConnectionFactory;
        public int FraId { get; set; }

        public Favourite(int fraId)
        {
            FraId = fraId;
        }
        public Favourite()
        {
        }
        public Favourite(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<(bool success, string message)> FavouriteFundraiser(int userId, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string favouriteFundraiserQuery = @"INSERT INTO FavouriteList (UserId, FraId) VALUES (@userId, @fraId)";
            try
            {
                using var favouriteFundraiserQueryCommand = new SqliteCommand(favouriteFundraiserQuery, connection, transaction);
                favouriteFundraiserQueryCommand.Parameters.AddWithValue("@userId", userId);
                favouriteFundraiserQueryCommand.Parameters.AddWithValue("@fraId", fraId);

                int rowsAffected = await favouriteFundraiserQueryCommand.ExecuteNonQueryAsync();
                if (rowsAffected == 1)
                {
                    transaction.Commit();
                    return (true, "Fundraiser Favourited");
                }
                transaction.Rollback();
                return (false, "failed to favourite fundraiser");
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
            {
                transaction.Rollback();
                return (false, "fundraiser already favourited");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int?> GetAmtOfFavourites(int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getAmtOfFavouritesQuery = @"SELECT Count(*) FROM FavouriteList Where FraId = @fraId";
            using var getAmtOfFavouritesQueryCommand = new SqliteCommand(getAmtOfFavouritesQuery, connection);
            getAmtOfFavouritesQueryCommand.Parameters.AddWithValue("@fraId", fraId);

            var amtOfFavourites = await getAmtOfFavouritesQueryCommand.ExecuteScalarAsync();
            if(amtOfFavourites != null)
            {
                return Convert.ToInt32(amtOfFavourites);
            }
            return null;

        }
        public async Task<List<FundraiserActivity>> GetFavouritesList(int userId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            List<FundraiserActivity> result = new List<FundraiserActivity>();

            string getFavouritesQuery = @"Select fra.* , fraC.FraCategoryName FROM FavouriteList FL INNER JOIN FundraiserActivity fra on fra.Id = FL.FraId JOIN FundraiserCategory fraC ON fra.FraCategoryId = fraC.Id WHERE UserId = @userId ";
            using var getFavouriteQueryCommand = new SqliteCommand(getFavouritesQuery, connection);
            getFavouriteQueryCommand.Parameters.AddWithValue("@userId", userId);

            var reader = await getFavouriteQueryCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                bool success = DateTime.TryParseExact(
                    reader.GetString(reader.GetOrdinal("Deadline")),
                    "o",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime readerDate
                );

                if (!success)
                {
                    throw new Exception("Invalid deadline format in database.");
                }
                result.Add(new FundraiserActivity(
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetString(reader.GetOrdinal("FraName")),
                            reader.GetString(reader.GetOrdinal("Description")),
                            readerDate.ToString("dd-MM-yyyy"),
                            reader.GetDouble(reader.GetOrdinal("AmtRequested")),
                            reader.GetDouble(reader.GetOrdinal("AmtDonated")),
                            reader.GetInt32(reader.GetOrdinal("AmtOfViews")),
                            reader.GetBoolean(reader.GetOrdinal("Status")),
                            reader.GetString(reader.GetOrdinal("FraCategoryName"))
                            ));
            }
            return result;
        }

        public async Task <List<FundraiserActivity>> SearchFavourites(string fraName, int userId)
        {
            List<FundraiserActivity> result = new List<FundraiserActivity>();

            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string SearchFavouriteQuery = @" SELECT fra.*, fraC.FraCategoryName 
                                                FROM FavouriteList FL 
                                                JOIN FundraiserActivity fra
                                                ON fra.Id = FL.FraId 
                                                JOIN FundraiserCategory fraC
                                                ON fra.FraCategoryId = fraC.Id
                                                WHERE fra.FraName LIKE '%'||@fraName||'%'
                                                AND FL.UserId = @userId";
            using var SearchFavouriteQueryCommand = new SqliteCommand(SearchFavouriteQuery, connection);
            SearchFavouriteQueryCommand.Parameters.AddWithValue("@fraName", fraName);
            SearchFavouriteQueryCommand.Parameters.AddWithValue("@userId", userId);

            var reader = await SearchFavouriteQueryCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bool success = DateTime.TryParseExact(
                    reader.GetString(reader.GetOrdinal("Deadline")),
                    "o",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime readerDate
                );

                if (!success)
                {
                    throw new Exception("Invalid deadline format in database.");
                }
                result.Add(new FundraiserActivity(
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetString(reader.GetOrdinal("FraName")),
                            reader.GetString(reader.GetOrdinal("Description")),
                            readerDate.ToString("dd-MM-yyyy"),
                            reader.GetDouble(reader.GetOrdinal("AmtRequested")),
                            reader.GetDouble(reader.GetOrdinal("AmtDonated")),
                            reader.GetInt32(reader.GetOrdinal("AmtOfViews")),
                            reader.GetBoolean(reader.GetOrdinal("Status")),
                            reader.GetString(reader.GetOrdinal("FraCategoryName"))
                            ));
            }
            return result;

        }


        public async Task<bool> UnfavouriteFundraiser(int userId, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            string unfavouriteFundraiserQuery = @"DELETE FROM FavouriteList WHERE UserId = @userId and FraId = @fraId";
            using var unfavouriteFundraiserQueryCommand = new SqliteCommand(unfavouriteFundraiserQuery, connection, (SqliteTransaction)transaction);
            unfavouriteFundraiserQueryCommand.Parameters.AddWithValue("@userId", userId);
            unfavouriteFundraiserQueryCommand.Parameters.AddWithValue("@fraId", fraId);

            int rowsAffected = await unfavouriteFundraiserQueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }
    }
}
