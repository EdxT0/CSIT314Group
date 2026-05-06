using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace CSIT_314_Group.Data
{
    public class UserAccount
    {
        private readonly DbConnectionFactory _dbConnectionFactory;


        public int Id { get;  set; }
        public string Name { get;  set; } = "";
        public string Email { get;  set; } = "";
        public string PhoneNumber { get;  set; } = "";
        public string HashedPassword { get;  set; } = "";
        public int ProfileId { get;  set; }
        public bool IsSuspended { get;  set; } = false;
        public string ProfileName { get; set; } = "";

        public UserAccount(
            string name,
            string email,
            string phoneNumber,
            string hashedPassword, 
            int profileId,
            bool isSuspended)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            ProfileId = profileId;
            IsSuspended = isSuspended;
            HashedPassword = hashedPassword;
        }
        public UserAccount(
            string name,
            string email,
            string phoneNumber,
            string hashedPassword,
            string profileName,
            bool isSuspended)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            ProfileName = profileName;
            IsSuspended = isSuspended;
            HashedPassword = hashedPassword;
        }
        public UserAccount(
            string name,
            string email,
            string phoneNumber,
            int profileId,
            bool isSuspended
            )
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            ProfileId = profileId;
            IsSuspended = isSuspended;
        }
        public UserAccount(
           int id,
           string name,
           string email,
           string phoneNumber,
           string hashedPassword,
           int profileId,
           bool isSuspended)
        {
            Id = id;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            HashedPassword = hashedPassword;
            ProfileId = profileId;
            IsSuspended = isSuspended;
        }




        public UserAccount(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public UserAccount()
        {

        }

        public void UpdateContactDetails(string name, string email, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public void setPassword(string newHashedPassword)
        {
            if (string.IsNullOrWhiteSpace(newHashedPassword))
                throw new ArgumentException("Password hash cannot be empty.");

            HashedPassword = newHashedPassword;
        }

        public async Task<UserAccount> GetById(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            string getByIdQuery = @"SELECT UA.NAME, UA.PhoneNumber, UA.Email, UP.ProfileName, UA.IsSuspended FROM UserAccount UA JOIN UserProfile UP on UA.ProfileId = UP.Id WHERE UA.Id = @id";
            using var getByIdQueryCommand = new SqliteCommand(getByIdQuery, connection);
            getByIdQueryCommand.Parameters.AddWithValue("@id", id);
            var reader = await getByIdQueryCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserAccount
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    ProfileName = reader.GetString(reader.GetOrdinal("ProfileName")),
                    IsSuspended = reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                };
            }
            return null;
        }

        public async Task<UserAccount>? GetByEmail(string email)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                string getByEmailQuery = @"SELECT * FROM UserAccount WHERE Email = @email";

                await using var getByEmailQueryCommand = new SqliteCommand(getByEmailQuery, connection);
                getByEmailQueryCommand.Parameters.AddWithValue("@email", email);

                var reader = await getByEmailQueryCommand.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserAccount
                    (
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.GetString(reader.GetOrdinal("Email")),
                        reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        reader.GetString(reader.GetOrdinal("HashedPassword")),
                        reader.GetInt32(reader.GetOrdinal("ProfileId")),
                        reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                    );
                }
                return null;

            }
            catch
            {

                throw;
            }

        }

        public async Task<(bool success, string message)> CreateUser(UserAccount userDetails)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();
            try
            {
                string createUserQuery = @"INSERT INTO UserAccount ( Name, PhoneNumber, Email, HashedPassword, ProfileId, IsSuspended) VALUES ( @Name, @PhoneNumber, @Email, @HashedPassword, @ProfileId, @IsSuspended)";

                await using var createUserQueryCommand = new SqliteCommand(createUserQuery, connection, transaction);

                createUserQueryCommand.Parameters.AddWithValue("@Name", userDetails.Name);
                createUserQueryCommand.Parameters.AddWithValue("@PhoneNumber", userDetails.PhoneNumber);
                createUserQueryCommand.Parameters.AddWithValue("@Email", userDetails.Email);
                createUserQueryCommand.Parameters.AddWithValue("@HashedPassword", userDetails.HashedPassword);
                createUserQueryCommand.Parameters.AddWithValue("@ProfileId", userDetails.ProfileId);
                createUserQueryCommand.Parameters.AddWithValue("@IsSuspended", userDetails.IsSuspended);


                int rowsAffected = await createUserQueryCommand.ExecuteNonQueryAsync();


                if (rowsAffected != 1)
                {
                    await transaction.RollbackAsync();
                    return (false,"failed to create user");
                }
                await transaction.CommitAsync();
                return (true, "user succesfully created");
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
            {
                if (ex.Message.Contains("UserAccount.email", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "email exists already");
                }
                if (ex.Message.Contains("UserAccount.phoneNumber", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, "phone number exists already");
                }
                return (false, "Both phone number and email exists already");
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return (false, "failed to create user");
            }

        }

        public async Task<UserAccount> ViewUserAccount(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            using SqliteTransaction transaction = connection.BeginTransaction();

            string viewUserAccountQuery = @"SELECT UA.Id, UA.Name, UA.Email, UA.PhoneNumber, UP.ProfileName, UA.IsSuspended FROM UserAccount UA Join UserProfile UP ON UA.ProfileId = UP.Id  WHERE UA.id = @id";

            using var viewUserAccountQueryCommand = new SqliteCommand(viewUserAccountQuery, connection, transaction);
            viewUserAccountQueryCommand.Parameters.AddWithValue("@id", id);

            var reader = await viewUserAccountQueryCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserAccount
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    ProfileName = reader.GetString(reader.GetOrdinal("ProfileName")),
                    IsSuspended = reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                };
            }
            return null;
        }

        public async Task<List<UserAccount>> ViewAllUserAccount()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string viewAllUserAccountQuery = @"SELECT UA.Id, UA.Name, UA.Email, UA.PhoneNumber, UP.ProfileName, UA.IsSuspended FROM UserAccount UA JOIN USERPROFILE UP ON UA.ProfileID = UP.Id";
            var viewAllUserAccountQueryCommand = new SqliteCommand(viewAllUserAccountQuery, connection);
            using var reader = await viewAllUserAccountQueryCommand.ExecuteReaderAsync();
            List<UserAccount> listOfAllUserAccount = new List<UserAccount>();

            while (await reader.ReadAsync())
            {
                listOfAllUserAccount.Add(new UserAccount
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    ProfileName = reader.GetString(reader.GetOrdinal("ProfileName")),
                    IsSuspended = reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                });
            }
            return listOfAllUserAccount;
        }
        public async Task<bool> SuspendUserWithId(int id, bool suspendUser)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string suspendUserWithIdQuery = @"UPDATE UserAccount SET IsSuspended = @suspendUser WHERE Id = @id";
            using var suspendUserWithIdQueryCommand = new SqliteCommand(suspendUserWithIdQuery, connection, transaction);
            suspendUserWithIdQueryCommand.Parameters.AddWithValue("@suspendUser", suspendUser);
            suspendUserWithIdQueryCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await suspendUserWithIdQueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }
            await transaction.CommitAsync();
            return true;
        }

        public async Task<List<UserAccount>> GetAllWithQuery(string query)
        {
            List<UserAccount> listOfAllUserAccount = new();

            using var connection = new SqliteConnection("data source = app.db");
            await connection.OpenAsync();

            string GetAllWithQuery = "" +
                "SELECT * FROM UserAccount JOIN UserProfile ON UserAccount.ProfileId = UserProfile.Id " +
                "WHERE Name LIKE '%' || @query || '%'" +
                " OR PhoneNumber LIKE '%' || @query || '%'" +
                " OR Email LIKE '%' || @query || '%'";
            using var GetAllWithQueryCommand = new SqliteCommand(GetAllWithQuery, connection);
            GetAllWithQueryCommand.Parameters.AddWithValue("@query", query);

            var reader = await GetAllWithQueryCommand.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                listOfAllUserAccount.Add(new UserAccount
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    ProfileName = reader.GetString(reader.GetOrdinal("ProfileName")),
                    IsSuspended = reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                });
            }
            return listOfAllUserAccount;
        }
        
        public async Task<bool> GetSuspendStatusWithId(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            string getSuspendStatusWithIdQuery = @"SELECT IsSuspended FROM UserAccount WHERE Id = @id";
            using var getSuspendStatusWithIdQueryCommand = new SqliteCommand(getSuspendStatusWithIdQuery, connection);
            getSuspendStatusWithIdQueryCommand.Parameters.AddWithValue("@id", id);

            var result = await getSuspendStatusWithIdQueryCommand.ExecuteScalarAsync();
            bool isSuspended = Convert.ToBoolean(result);
            return isSuspended;
        }

        public async Task<List<int>> GetIdsWithQuery(string nameOrEmailOrPhone)
        {
            List<int> ids = new List<int>();

            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"
                            SELECT Id 
                            FROM UserAccount
                            WHERE Name LIKE '%' || @search || '%'
                            OR Email LIKE '%' || @search || '%'
                            OR PhoneNumber LIKE '%' || @search || '%'";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@search", nameOrEmailOrPhone);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ids.Add(reader.GetInt32(0));
            }

            return ids;
        }

        public async Task<bool> UpdateEmailById(int id, string email)
        {
            using SqliteConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string updateEmailByIdQuery = @"UPDATE UserAccount Set Email = @email Where Id = @id";
            using var updateEmailByIdQueryCommand = new SqliteCommand(updateEmailByIdQuery, connection, transaction);
            updateEmailByIdQueryCommand.Parameters.AddWithValue("@email", email);
            updateEmailByIdQueryCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await updateEmailByIdQueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;



        }
        public async Task<bool> UpdatePhoneNumberById(int id, string phoneNumber)
        {
            using SqliteConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string UpdatePhoneNumberByIdQuery = @"UPDATE UserAccount Set PhoneNumber = @phoneNumber Where Id = @id";
            using var UpdatePhoneNumberByIdQueryCommand = new SqliteCommand(UpdatePhoneNumberByIdQuery, connection, transaction);
            UpdatePhoneNumberByIdQueryCommand.Parameters.AddWithValue("@phoneNumber", phoneNumber);
            UpdatePhoneNumberByIdQueryCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await UpdatePhoneNumberByIdQueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;


        }
        public async Task<bool> UpdateNameById(int id, string name)
        {
            using SqliteConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string UpdateNameByIdQuery = @"UPDATE UserAccount Set Name = @name Where Id = @id";
            using var UpdateNameByIdCommand = new SqliteCommand(UpdateNameByIdQuery, connection, transaction);
            UpdateNameByIdCommand.Parameters.AddWithValue("@name", name);
            UpdateNameByIdCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await UpdateNameByIdCommand.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;

        }
        public async Task<bool> UpdatePasswordById(int id, string password)
        {
            using SqliteConnection connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string UpdatePasswordByIdQuery = @"UPDATE UserAccount Set HashedPassword = @password Where Id = @id";
            using var UpdatePasswordByIdQueryCommand = new SqliteCommand(UpdatePasswordByIdQuery, connection, transaction);
            UpdatePasswordByIdQueryCommand.Parameters.AddWithValue("@password", password);
            UpdatePasswordByIdQueryCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await UpdatePasswordByIdQueryCommand.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;
        }
        public async Task<bool> UpdateProfileById(int id, int? profileId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            string UpdateNameByIdQuery = @"UPDATE UserAccount Set ProfileId = @profileId Where Id = @id";
            using var UpdateNameByIdCommand = new SqliteCommand(UpdateNameByIdQuery, connection, transaction);
            UpdateNameByIdCommand.Parameters.AddWithValue("@profileId", profileId);
            UpdateNameByIdCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await UpdateNameByIdCommand.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                await transaction.CommitAsync();
                return true;
            }

            await transaction.RollbackAsync();
            return false;
        }

        public async Task<UserAccount>? GetAllDetailsById(int userId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            try
            {
                string getByEmailQuery = @"SELECT * FROM UserAccount WHERE Id = @userId";

                await using var getByEmailQueryCommand = new SqliteCommand(getByEmailQuery, connection);
                getByEmailQueryCommand.Parameters.AddWithValue("@userId", userId);

                var reader = await getByEmailQueryCommand.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserAccount
                     (
                      reader.GetInt32(reader.GetOrdinal("Id")),
                      reader.GetString(reader.GetOrdinal("Name")),
                      reader.GetString(reader.GetOrdinal("Email")),
                      reader.GetString(reader.GetOrdinal("PhoneNumber")),
                      reader.GetString(reader.GetOrdinal("HashedPassword")),
                      reader.GetInt32(reader.GetOrdinal("ProfileId")),
                      reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                     );
                }
                return null;

            }
            catch
            {

                throw;
            }

        }
    }
}
