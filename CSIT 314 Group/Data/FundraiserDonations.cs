
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CSIT_314_Group.Data
{
    public class FundraiserDonations
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        public int Id { get; set; }
        public double UserDonatedAmt { get;  set; }
        public string? DateDonated { get;  set; }
        public string? Name { get;  set; }
        public string? Description { get;  set; }
        public string? Deadline { get;  set; }
        public bool Status { get;  set; }
        public double AmtRequested { get;  set; }
        public double AmtDonated { get;  set; }
        public int AmtOfViews { get;  set; }

        public string? FraCategoryName { get;  set; }
        public FundraiserDonations()
        {
        }
        public FundraiserDonations(int id, int userDonatedAmount, string? dateDonated, string? name, string? description, string? deadline, double amtRequested, double amtDonated, int amtOfViews, bool status, string? fraCategoryName)
        {
            Id = id;
            DateDonated = dateDonated;
            UserDonatedAmt = userDonatedAmount;
            Name = name;
            Description = description;
            Deadline = deadline;
            AmtRequested = amtRequested;
            AmtDonated = amtDonated;
            AmtOfViews = amtOfViews;
            Status = status;
            FraCategoryName = fraCategoryName;
        }
    

        public FundraiserDonations(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<bool> DeleteDonation(int userId, int fraId, double amtDonated, DateTime dateDonated)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            string deleteDonationquery = @"DELETE FROM FundraiserDonations WHERE UserId = @userId AND FraId = @fraId AND AmtDonatedByUser = @amtDonated AND DateDonated = @dateDonated";
            using var deleteDonationqueryCommand = new SqliteCommand(deleteDonationquery, connection, (SqliteTransaction)transaction);
            deleteDonationqueryCommand.Parameters.AddWithValue("@userId", userId);
            deleteDonationqueryCommand.Parameters.AddWithValue("@fraId", fraId);
            deleteDonationqueryCommand.Parameters.AddWithValue("@amtDonated", amtDonated);
            deleteDonationqueryCommand.Parameters.AddWithValue("@dateDonated", dateDonated.ToString("O"));

            int rowsAffected = await deleteDonationqueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }
        public async Task<bool> AddDonation(int userId, int fraId, double amtDonated, DateTime dateDonated)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            string addDonationQuery = @"INSERT INTO FundraiserDonations (UserId, FraId, AmtDonatedByUser, DateDonated) VALUES (@userId, @fraId, @amtDonated, @dateDonated)";
            using var addDonationQueryCommand = new SqliteCommand(addDonationQuery, connection, (SqliteTransaction)transaction);

            addDonationQueryCommand.Parameters.AddWithValue("@userId", userId);
            addDonationQueryCommand.Parameters.AddWithValue("@fraId", fraId);
            addDonationQueryCommand.Parameters.AddWithValue("@amtDonated", amtDonated);
            addDonationQueryCommand.Parameters.AddWithValue("@dateDonated", dateDonated.ToString("O"));

            int rowsAffected = await addDonationQueryCommand.ExecuteNonQueryAsync();

            if(rowsAffected == 1)
            {
                await transaction.CommitAsync();
                return true;
            }
            await transaction.RollbackAsync();
            return false;
        }

        public async Task<List<FundraiserDonations>> ViewDonatedFundraiser(int userId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            List<FundraiserDonations> result = new List<FundraiserDonations>();
            string viewDonatedFundraiserQuery = @"SELECT fra.*, fraC.FraCategoryName , fraD.*
                                                    FROM FundraiserDonations fraD 
                                                    JOIN FundraiserActivity fra 
                                                    ON fraD.FraId = fra.Id 
                                                    JOIN FundraiserCategory fraC 
                                                    ON fra.FraCategoryId = fraC.Id  
                                                    WHERE fraD.UserId = @userId ";
            using var viewDonatedFundraiserQueryCommand = new SqliteCommand(viewDonatedFundraiserQuery, connection);
            viewDonatedFundraiserQueryCommand.Parameters.AddWithValue("@userId", userId);

            var reader = await viewDonatedFundraiserQueryCommand.ExecuteReaderAsync();
            while(await reader.ReadAsync())
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

                bool successForDonationDate = DateTime.TryParseExact(
                    reader.GetString(reader.GetOrdinal("DateDonated")),
                    "o",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime DonationDate
                );
                if (!successForDonationDate)
                {
                    throw new Exception("Invalid deadline format in database.");
                }
                result.Add(new FundraiserDonations(
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetInt32(reader.GetOrdinal("AmtDonatedByUser")),
                            DonationDate.ToString("dd-MM-yyyy"),
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

        public async Task<List<FundraiserDonations>> SearchDonations(string fraName, int userId)
        {
            List<FundraiserDonations> result = new List<FundraiserDonations>();

            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string searchDonationsQuery = @" SELECT fra.*, fraC.FraCategoryName , fraD.*
                                                FROM FundraiserDonations fraD 
                                                JOIN FundraiserActivity fra
                                                ON fra.Id = fraD.FraId 
                                                JOIN FundraiserCategory fraC
                                                ON fra.FraCategoryId = fraC.Id
                                                WHERE fra.FraName LIKE '%'||@fraName||'%'
                                                AND fraD.UserId = @userId";
            using var searchDonationsQueryCommand = new SqliteCommand(searchDonationsQuery, connection);
            searchDonationsQueryCommand.Parameters.AddWithValue("@fraName", fraName);
            searchDonationsQueryCommand.Parameters.AddWithValue("@userId", userId);

            var reader = await searchDonationsQueryCommand.ExecuteReaderAsync();

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

                bool successForDonationDate = DateTime.TryParseExact(
                    reader.GetString(reader.GetOrdinal("DateDonated")),
                    "o",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime DonationDate
                );
                if (!successForDonationDate)
                {
                    throw new Exception("Invalid deadline format in database.");
                }
                result.Add(new FundraiserDonations(
                            reader.GetInt32(reader.GetOrdinal("Id")),
                            reader.GetInt32(reader.GetOrdinal("AmtDonatedByUser")),
                            DonationDate.ToString("dd-MM-yyyy"),
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
