using CSIT_314_Group.DTO.FundraiserActivityDTO;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CSIT_314_Group.Data
{
    public class UserFundraiserRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public UserFundraiserRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> AddFRAToUser(int userId, int? fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string addFRAtoUserQuery = @"INSERT INTO UserFundraiser (UserId, FraId) VALUES (@userId, @fraId)";
            using var addFRAtoUserQueryCommand = new SqliteCommand(addFRAtoUserQuery, connection, transaction);
            addFRAtoUserQueryCommand.Parameters.AddWithValue("@userId", userId);
            addFRAtoUserQueryCommand.Parameters.AddWithValue("@fraId", fraId);
            int rowsAffected = await addFRAtoUserQueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }

        public async Task<List<ViewFundraiserDTO>> ViewMyFundraisers(int userId)
        {
            List<ViewFundraiserDTO> result = new List<ViewFundraiserDTO>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string ViewMyFundraisersQuery = @"SELECT Fra.Id, Fra.FraName, Fra.Description, Fra.Deadline, Fra.Status, Fra.AmtOfViews, Fra.AmtDonated, Fra.AmtRequested 
                                            FROM UserFundraiser UF 
                                        INNER JOIN FundraiserActivity Fra ON Fra.Id = UF.FraId
                                        WHERE UF.UserID = @userId";

            using var ViewMyFundraisersQueryCommand = new SqliteCommand(ViewMyFundraisersQuery, connection);
            ViewMyFundraisersQueryCommand.Parameters.AddWithValue("@userId", userId);
            var reader = await ViewMyFundraisersQueryCommand.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                bool parsedDate = DateTime.TryParseExact(reader.GetString(reader.GetOrdinal("Deadline")),
                                                         "O",
                                                         CultureInfo.InvariantCulture,
                                                         DateTimeStyles.None,
                                                         out DateTime readerDate
                                                    );

                if (!parsedDate)
                {
                    throw new Exception("Invalid deadline format in database.");
                }
                result.Add(new ViewFundraiserDTO(
                           reader.GetInt32(reader.GetOrdinal("Id")),
                           reader.GetString(reader.GetOrdinal("FraName")),
                           reader.GetString(reader.GetOrdinal("Description")),
                           readerDate.ToString("dd-MM-yyyy"),
                           reader.GetDouble(reader.GetOrdinal("AmtRequested")),
                           reader.GetDouble(reader.GetOrdinal("AmtDonated")),
                           reader.GetInt32(reader.GetOrdinal("AmtOfViews")),
                           reader.GetBoolean(reader.GetOrdinal("Status"))
                           ));
            }
            return result;
        }
    }
}
