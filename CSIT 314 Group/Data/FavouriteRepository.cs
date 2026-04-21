using CSIT_314_Group.DTO.FundraiserActivityDTO;
using CSIT_314_Group.Results;
using Microsoft.Data.Sqlite;

namespace CSIT_314_Group.Data
{
    public class FavouriteRepository
    {

        private readonly DbConnectionFactory _dbConnectionFactory;

        public FavouriteRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<FavouriteFundraiserResultEnum> FavouriteFundraiser(int userId, int fraId)
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
                    return FavouriteFundraiserResultEnum.success;
                }
                transaction.Rollback();
                return FavouriteFundraiserResultEnum.failed;
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
            {
                transaction.Rollback();
                return FavouriteFundraiserResultEnum.duplicate;
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
        public async Task<List<ViewFundraiserDTO>> GetFavouritesList(int userId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            List<ViewFundraiserDTO> result = new List<ViewFundraiserDTO>();

            string getFavouritesQuery = @"Select fra.* FROM FavouriteList FL INNER JOIN FundraiserActivity fra on fra.Id = FL.FraId WHERE UserId = @userId ";
            using var getFavouriteQueryCommand = new SqliteCommand(getFavouritesQuery, connection);
            getFavouriteQueryCommand.Parameters.AddWithValue("@userId", userId);

            var reader = await getFavouriteQueryCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new ViewFundraiserDTO(reader.GetInt32(reader.GetOrdinal("Id")),
                                                 reader.GetString(reader.GetOrdinal("FraName")),
                                                 reader.GetString(reader.GetOrdinal("Description")),
                                                 reader.GetString(reader.GetOrdinal("Deadline")),
                                                 reader.GetDouble(reader.GetOrdinal("amtRequested")),
                                                 reader.GetDouble(reader.GetOrdinal("amtDonated")),
                                                 reader.GetInt32(reader.GetOrdinal("amtOfViews")),
                                                 reader.GetBoolean(reader.GetOrdinal("status"))
                                                 )
                    );
            }
            return result;
        }


    }
}
