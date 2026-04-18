using CSIT_314_Group.DTO.UserDTO;
using CSIT_314_Group.Entity;
using CSIT_314_Group.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace CSIT_314_Group.Data
{
    public class UserAccountRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        public UserAccountRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<UserAccountDTO> GetById(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            string getByIdQuery = @"SELECT UA.NAME, UA.PhoneNumber, UA.Email, UP.ProfileName, UA.IsSuspended FROM UserAccount UA JOIN UserProfile UP on UA.ProfileId = UP.Id WHERE UA.Id = @id";
            using var getByIdQueryCommand = new SqliteCommand(getByIdQuery, connection);
            getByIdQueryCommand.Parameters.AddWithValue("@id", id);
            var reader = await getByIdQueryCommand.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                return new UserAccountDTO
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
                    {
                        id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        ProfileId = reader.GetInt32(reader.GetOrdinal("ProfileId")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        HashedPassword = reader.GetString(reader.GetOrdinal("HashedPassword")),
                        IsSuspended = reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                    };
                }
                return null;

            }
            catch
            {
               
                throw;
            }

        }

        public async Task<CreateUserResultEnum> CreateUser(UserAccount userDetails)
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
                    return CreateUserResultEnum.Failed;
                }
                await transaction.CommitAsync();
                return CreateUserResultEnum.Success;
            }
            catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
            {
                if (ex.Message.Contains("UserAccount.email", StringComparison.OrdinalIgnoreCase))
                {
                    return CreateUserResultEnum.DuplicateEmail;
                }
                if (ex.Message.Contains("UserAccount.phoneNumber", StringComparison.OrdinalIgnoreCase))
                {
                    return CreateUserResultEnum.DuplicatePhoneNumber;
                }
                return CreateUserResultEnum.DuplicatePhoneNumberAndEmail;
            }
            catch(SqliteException ex)
            {
                Console.WriteLine(ex);
                await transaction.RollbackAsync();
                return CreateUserResultEnum.Failed;
            }

        }

        public async Task<UserAccountDTO> ViewUserAccount(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            using SqliteTransaction transaction = connection.BeginTransaction();

            string viewUserAccountQuery = @"SELECT UA.Id, UA.Name, UA.Email, UA.PhoneNumber, UP.ProfileName, UA.IsSuspended FROM UserAccount UA Join UserProfile UP ON UA.ProfileId = UP.Id  WHERE UA.id = @id";

            using var viewUserAccountQueryCommand = new SqliteCommand(viewUserAccountQuery,connection,transaction);
            viewUserAccountQueryCommand.Parameters.AddWithValue("@id", id);

            var reader = await viewUserAccountQueryCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserAccountDTO
                {
                    id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    ProfileName = reader.GetString(reader.GetOrdinal("ProfileName")),
                    IsSuspended = reader.GetBoolean(reader.GetOrdinal("IsSuspended"))
                };
            }
            return null;
        }

        public async Task<List<UserAccountDTO>> ViewAllUserAccount()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string viewAllUserAccountQuery = @"SELECT UA.Id, UA.Name, UA.Email, UA.PhoneNumber, UP.ProfileName, UA.IsSuspended FROM UserAccount UA JOIN USERPROFILE UP ON UA.ProfileID = UP.Id";
            var viewAllUserAccountQueryCommand = new SqliteCommand(viewAllUserAccountQuery, connection);
            using var reader = await viewAllUserAccountQueryCommand.ExecuteReaderAsync();
            List<UserAccountDTO> listOfAllUserAccount = new List<UserAccountDTO>();

            while (await reader.ReadAsync())
            {
                listOfAllUserAccount.Add(new UserAccountDTO
                {
                    id = reader.GetInt32(reader.GetOrdinal("Id")),
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

            if(rowsAffected != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }
            await transaction.CommitAsync();
            return true;
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

        public async Task<int?> GetIdWithNameOrEmailOrPhone(string nameOrEmailOrPhone)
        {
            object? result = null;
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
          

            string getIdWithNameQuery = @"SELECT id FROM UserAccount WHERE Name = @name";

            using var getIdWithNameQueryCommand = new SqliteCommand(getIdWithNameQuery, connection);
            getIdWithNameQueryCommand.Parameters.AddWithValue("@name", nameOrEmailOrPhone);

            result = await getIdWithNameQueryCommand.ExecuteScalarAsync();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }

            string getIdWithEmailQuery = @"SELECT id FROM UserAccount WHERE Email = @email";

            using var getIdWithEmailQueryCommand = new SqliteCommand(getIdWithEmailQuery, connection);
            getIdWithEmailQueryCommand.Parameters.AddWithValue("@email", nameOrEmailOrPhone);

            result = await getIdWithEmailQueryCommand.ExecuteScalarAsync();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }

            string getIdWithPhoneQuery = @"SELECT id FROM UserAccount WHERE PhoneNumber = @phone";

            using var getIdWithPhoneQueryCommand = new SqliteCommand(getIdWithPhoneQuery, connection);
            getIdWithEmailQueryCommand.Parameters.AddWithValue("@phone", nameOrEmailOrPhone);

            result = await getIdWithEmailQueryCommand.ExecuteScalarAsync();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            return null;

        }
    }
}
