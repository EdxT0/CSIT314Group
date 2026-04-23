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

        public async Task<double?> getAmtDonated(int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getAmtDonatedQuery = @"SELECT AmtDonated FROM FundraiserActivity WHERE Id = @fraId";
            using var getAmtDonatedQueryCommand = new SqliteCommand(getAmtDonatedQuery, connection);

            getAmtDonatedQueryCommand.Parameters.AddWithValue("@fraId", fraId);

            var amtRequested = await getAmtDonatedQueryCommand.ExecuteScalarAsync();

            if (amtRequested != null)
            {
                return Convert.ToDouble(amtRequested);
            }
            return null;

        }
        public async Task<double?> getAmtRequested(int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getAmtRequestedQuery = @"SELECT AmtRequested FROM FundraiserActivity WHERE Id = @fraId";
            using var getAmtRequestedQueryCommand = new SqliteCommand(getAmtRequestedQuery, connection);

            getAmtRequestedQueryCommand.Parameters.AddWithValue("@fraId", fraId);

            var amtRequested = await getAmtRequestedQueryCommand.ExecuteScalarAsync();

            if (amtRequested != null)
            {
                return Convert.ToDouble(amtRequested);
            }
            return null;

        }
        public async Task<bool> updateName(string name, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateNameQuery = @"UPDATE FundraiserActivity SET FraName = @name where Id = @id";
            using var updateNameQueryCommand = new SqliteCommand(updateNameQuery, connection, transaction);
            updateNameQueryCommand.Parameters.AddWithValue("@name", name);
            updateNameQueryCommand.Parameters.AddWithValue("@id", fraId);

            int rowsAffected = await updateNameQueryCommand.ExecuteNonQueryAsync();
            if(rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }

        public async Task<bool> UpdateFundraiserView(int fundraiserId)
        {
            ViewFundraiserDTO fundraiser = await GetById(fundraiserId);
            int viewsAfterIncrement = 1 + fundraiser.AmtOfViews;
            if(fundraiser != null)
            {
                using var connection = _dbConnectionFactory.CreateConnection();
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                string updateFundraiserViewQuery = @"UPDATE FundraiserActivity SET AmtOfViews = @amtOfViews WHERE Id = @fraId";
                using var updateFundraiserViewQueryCommand = new SqliteCommand(updateFundraiserViewQuery, connection, transaction);
                updateFundraiserViewQueryCommand.Parameters.AddWithValue("@amtOfViews", viewsAfterIncrement);
                updateFundraiserViewQueryCommand.Parameters.AddWithValue("@fraId", fundraiserId);

                int rowsAffected = await updateFundraiserViewQueryCommand.ExecuteNonQueryAsync();

                if(rowsAffected == 1)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                await transaction.RollbackAsync();
            }
            return false;
        }

        public async Task<bool> DeleteFundraiser(int fundraiserId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string deleteFundraiserQuery = @"DELETE FROM FundraiserActivity WHERE Id = @fraId ";
            using var deleteFundraiserQuerycommand = new SqliteCommand(deleteFundraiserQuery, connection, transaction);
            deleteFundraiserQuerycommand.Parameters.AddWithValue("@fraId", fundraiserId);

            int rowsAffected = await deleteFundraiserQuerycommand.ExecuteNonQueryAsync();

            if(rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }

        public async Task<bool> UpdateDesc(string description, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateDescQuery = @"UPDATE FundraiserActivity SET Description = @description where Id = @id";
            using var updateDescQueryCommand = new SqliteCommand(updateDescQuery, connection, transaction);
            updateDescQueryCommand.Parameters.AddWithValue("@description", description);
            updateDescQueryCommand.Parameters.AddWithValue("@id", fraId);

            int rowsAffected = await updateDescQueryCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }
        public async Task<bool> UpdateStatus(bool? status, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateStatusQuery = @"UPDATE FundraiserActivity SET Status = @status where Id = @id";
            using var updateStatusQueryCommand = new SqliteCommand(updateStatusQuery, connection, transaction);
            updateStatusQueryCommand.Parameters.AddWithValue("@status", status);
            updateStatusQueryCommand.Parameters.AddWithValue("@id", fraId);

            int rowsAffected = await updateStatusQueryCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;

        }
        public async Task<bool> UpdateDeadline(string deadline, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateDeadlineQuery = @"UPDATE FundraiserActivity SET Deadline = @deadline where Id = @id";
            using var updateDeadlineQueryCommand = new SqliteCommand(updateDeadlineQuery, connection, transaction);
            updateDeadlineQueryCommand.Parameters.AddWithValue("@deadline", deadline);
            updateDeadlineQueryCommand.Parameters.AddWithValue("@id", fraId);

            int rowsAffected = await updateDeadlineQueryCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }
        public async Task<bool> UpdateAmtRequested(double? amtRequested, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateAmtRequestedQuery = @"UPDATE FundraiserActivity SET AmtRequested = @amtRequested where Id = @id";
            using var updateAmtRequestedQueryCommand = new SqliteCommand(updateAmtRequestedQuery, connection, transaction);
            updateAmtRequestedQueryCommand.Parameters.AddWithValue("@amtRequested", amtRequested);
            updateAmtRequestedQueryCommand.Parameters.AddWithValue("@id", fraId);

            int rowsAffected = await updateAmtRequestedQueryCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }
        public async Task<bool> UpdateAmtDonated(double? amtDonated, int fraId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateAmtDonatedQuery = @"UPDATE FundraiserActivity SET AmtDonated = @amtDonated where Id = @id";
            using var updateAmtDonatedQueryCommand = new SqliteCommand(updateAmtDonatedQuery, connection, transaction);
            updateAmtDonatedQueryCommand.Parameters.AddWithValue("@amtDonated", amtDonated);
            updateAmtDonatedQueryCommand.Parameters.AddWithValue("@id", fraId);

            int rowsAffected = await updateAmtDonatedQueryCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }

        public async Task<ViewFundraiserDTO> GetByName(string name)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getByNameQuery = @"SELECT fra.*, fraC.FraCategoryName FROM FundraiserActivity fra JOIN FundraiserCategory fraC ON fra.FraCategoryId = fraC.Id WHERE fra.FraName = @name";
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
                            reader.GetBoolean(reader.GetOrdinal("Status")),
                            reader.GetString(reader.GetOrdinal("FraCategoryName"))
                            );
            }
            return null;
        }

        public async Task<ViewFundraiserDTO> GetById(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string getByIdQuery = @"SELECT fra.*, fraC.FraCategoryName FROM FundraiserActivity  fra JOIN FundraiserCategory fraC ON fra.FraCategoryId = fraC.Id WHERE fra.Id = @id";
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
                            reader.GetBoolean(reader.GetOrdinal("Status")),
                            reader.GetString(reader.GetOrdinal("FraCategoryName"))
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
                string createFundraiserQuery = @"INSERT INTO FundraiserActivity (FraName, Description, Deadline, Status, AmtOfViews, AmtDonated, AmtRequested, FraCategoryId ) VALUES (@fraName, @description, @deadline, @status, @amtOfViews, @amtDonated, @amtRequested, @fraCategoryId)";
                using var createFundraiserQueryCommand = new SqliteCommand(createFundraiserQuery, connection, transaction);

                createFundraiserQueryCommand.Parameters.AddWithValue("@fraName", fundraiser.Name);
                createFundraiserQueryCommand.Parameters.AddWithValue("@description", fundraiser.Description);
                createFundraiserQueryCommand.Parameters.AddWithValue("@deadline", fundraiser.Deadline.ToString("O"));
                createFundraiserQueryCommand.Parameters.AddWithValue("@status", fundraiser.Status);
                createFundraiserQueryCommand.Parameters.AddWithValue("@amtOfViews", fundraiser.AmtOfViews);
                createFundraiserQueryCommand.Parameters.AddWithValue("@amtDonated", fundraiser.AmtDonated);
                createFundraiserQueryCommand.Parameters.AddWithValue("@amtRequested", fundraiser.AmtRequested); 
                createFundraiserQueryCommand.Parameters.AddWithValue("@fraCategoryId", fundraiser.FraCategoryId);

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

        public async Task<List<ViewFundraiserDTO>> ViewAllFundraisers()
        {
            List<ViewFundraiserDTO> result = new List<ViewFundraiserDTO>();
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string viewAllFundraiserQuery = @"SELECT fra.*, fraC.FraCategoryName From FundraiserActivity fra JOIN FundraiserCategory fraC on fra.FraCategoryId = fraC.Id ";
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
                            reader.GetBoolean(reader.GetOrdinal("Status")),
                            reader.GetString(reader.GetOrdinal("FraCategoryName"))
                            ));

            }
            return result;
        }
    }
}
