using CSIT_314_Group.DTO.FundraiserActivityDTO;
using CSIT_314_Group.Entity;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CSIT_314_Group.Data
{
    public class FundraiserActivityRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public FundraiserActivityRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<ViewFundraiserDTO> GetByName(string name)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getByNameQuery = @"SELECT * FROM FundraiserActivity WHERE FraName = @name";
            using var getByNameQueryCommand = new SqliteCommand(getByNameQuery, connection);

            getByNameQueryCommand.Parameters.AddWithValue("@name", name);

            var reader = await getByNameQueryCommand.ExecuteReaderAsync();

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
                return new ViewFundraiserDTO(
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetString(reader.GetOrdinal("FraName")),
                            reader.GetString(reader.GetOrdinal("Description")),
                            readerDate.ToString("dd-MM-yyyy"),
                            reader.GetDouble(reader.GetOrdinal("AmtRequested")),
                            reader.GetDouble(reader.GetOrdinal("AmtDonated")),
                            reader.GetInt32(reader.GetOrdinal("AmtOfViews")),
                            reader.GetBoolean(reader.GetOrdinal("Status"))
                    );
            }
            return null;
        }
        
        public async Task<ViewFundraiserDTO?> GetById(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getByIdQuery = @"SELECT * FROM FundraiserActivity WHERE Id = @id";
            using var getByIdQueryCommand = new SqliteCommand(getByIdQuery, connection);

            getByIdQueryCommand.Parameters.AddWithValue("@id", id);

            var reader = await getByIdQueryCommand.ExecuteReaderAsync();

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

                return new ViewFundraiserDTO(
                    reader.GetInt32(reader.GetOrdinal("Id")),
                    reader.GetString(reader.GetOrdinal("FraName")),
                    reader.GetString(reader.GetOrdinal("Description")),
                    readerDate.ToString("dd-MM-yyyy"),
                    reader.GetDouble(reader.GetOrdinal("AmtRequested")),
                    reader.GetDouble(reader.GetOrdinal("AmtDonated")),
                    reader.GetInt32(reader.GetOrdinal("AmtOfViews")),
                    reader.GetBoolean(reader.GetOrdinal("Status"))
                );
            }

            return null;
        }

        public async Task<int?> createFundraiser(Fundraiser fundraiser)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int? fraId = null;
                string createFundraiserQuery = @"INSERT INTO FundraiserActivity (FraName, Description, Deadline, Status, AmtOfViews, AmtDonated, AmtRequested ) VALUES (@fraName, @description, @deadline, @status, @amtOfViews, @amtDonated, @amtRequested)";
                using var createFundraiserQueryCommand = new SqliteCommand(createFundraiserQuery, connection, transaction);

                createFundraiserQueryCommand.Parameters.AddWithValue("@fraName", fundraiser.Name);
                createFundraiserQueryCommand.Parameters.AddWithValue("@description", fundraiser.Description);
                createFundraiserQueryCommand.Parameters.AddWithValue("@deadline", fundraiser.Deadline.ToString("O"));
                createFundraiserQueryCommand.Parameters.AddWithValue("@status", fundraiser.Status);
                createFundraiserQueryCommand.Parameters.AddWithValue("@amtOfViews", fundraiser.AmtOfViews);
                createFundraiserQueryCommand.Parameters.AddWithValue("@amtDonated", fundraiser.AmtDonated);
                createFundraiserQueryCommand.Parameters.AddWithValue("@amtRequested", fundraiser.AmtRequested);

                int rowsAffected = await createFundraiserQueryCommand.ExecuteNonQueryAsync();

                if (rowsAffected == 1)
                {
                    await transaction.CommitAsync();
                    string getIdquery = @"select Id FROM FundraiserActivity WHERE FraName = @fraName";
                    using (var getIdQueryCommand = new SqliteCommand(getIdquery, connection))
                    {
                        getIdQueryCommand.Parameters.AddWithValue("@fraName", fundraiser.Name);
                        object? result = await getIdQueryCommand.ExecuteScalarAsync();
                        if (result != null)
                        {
                            fraId = Convert.ToInt16(result);
                        }
                    }
                    return fraId;
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return null;
            }
            await transaction.RollbackAsync();
            return null;

        }
        
        public async Task<bool> UpdateFundraiser(Fundraiser fundraiser)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string updateFundraiserQuery = @"
                UPDATE FundraiserActivity
                SET FraName = @fraName,
                Description = @description,
                Deadline = @deadline,
                AmtRequested = @amtRequested,
                Status = @status
                WHERE Id = @id";

            using var updateFundraiserQueryCommand = new SqliteCommand(updateFundraiserQuery, connection);

            updateFundraiserQueryCommand.Parameters.AddWithValue("@id", fundraiser.Id);
            updateFundraiserQueryCommand.Parameters.AddWithValue("@fraName", fundraiser.Name);
            updateFundraiserQueryCommand.Parameters.AddWithValue("@description", fundraiser.Description);
            updateFundraiserQueryCommand.Parameters.AddWithValue("@deadline", fundraiser.Deadline.ToString("O"));
            updateFundraiserQueryCommand.Parameters.AddWithValue("@amtRequested", fundraiser.AmtRequested);
            updateFundraiserQueryCommand.Parameters.AddWithValue("@status", fundraiser.Status);

            int rowsAffected = await updateFundraiserQueryCommand.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        public async Task<List<ViewFundraiserDTO>> ViewAllFundraisers()
        {
            List<ViewFundraiserDTO> result = new List<ViewFundraiserDTO>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string viewAllFundraiserQuery = @"SELECT * From FundraiserActivity";
            using var viewAllFundraiserQueryCommand = new SqliteCommand(viewAllFundraiserQuery, connection);
            var reader = await viewAllFundraiserQueryCommand.ExecuteReaderAsync();


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
