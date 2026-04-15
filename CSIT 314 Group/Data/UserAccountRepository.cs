using CSIT_314_Group.DTO;
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

            string getByIdQuery = @"SELECT * FROM user WHERE Id = @id";
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
                    Profile = reader.GetString(reader.GetOrdinal("Profile"))
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
                string getByEmailQuery = @"SELECT * FROM user WHERE Email = @email";

                await using var getByEmailQueryCommand = new SqliteCommand(getByEmailQuery, connection);
                getByEmailQueryCommand.Parameters.AddWithValue("@email", email);

                var reader = await getByEmailQueryCommand.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new UserAccount
                    {
                        id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Profile = reader.GetString(reader.GetOrdinal("Profile")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        HashedPassword = reader.GetString(reader.GetOrdinal("HashedPassword"))
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
                string createUserQuery = @"INSERT INTO user ( Name, PhoneNumber, Email, HashedPassword, Profile) VALUES ( @Name, @PhoneNumber, @Email, @HashedPassword, @Profile)";

                await using var createUserQueryCommand = new SqliteCommand(createUserQuery, connection, transaction);

                createUserQueryCommand.Parameters.AddWithValue("@Name", userDetails.Name);
                createUserQueryCommand.Parameters.AddWithValue("@PhoneNumber", userDetails.PhoneNumber);
                createUserQueryCommand.Parameters.AddWithValue("@Email", userDetails.Email);
                createUserQueryCommand.Parameters.AddWithValue("@HashedPassword", userDetails.HashedPassword);
                createUserQueryCommand.Parameters.AddWithValue("@Profile", userDetails.Profile);


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
                if (ex.Message.Contains("user.email", StringComparison.OrdinalIgnoreCase))
                {
                    return CreateUserResultEnum.DuplicateEmail;
                }
                if (ex.Message.Contains("user.phoneNumber", StringComparison.OrdinalIgnoreCase))
                {
                    return CreateUserResultEnum.DuplicatePhoneNumber;
                }
                return CreateUserResultEnum.DuplicatePhoneNumberAndEmail;
            }
            catch
            {
                await transaction.RollbackAsync();
                return CreateUserResultEnum.Failed;
            }

        }

        public async Task<UserAccountDTO> ViewUserAccount(int id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            using SqliteTransaction transaction = connection.BeginTransaction();

            string viewUserAccountQuery = @"SELECT * FROM user WHERE id = @id";

            using var viewUserAccountQueryCommand = new SqliteCommand(viewUserAccountQuery,connection,transaction);
            viewUserAccountQueryCommand.Parameters.AddWithValue("@id", id);

            var reader = await viewUserAccountQueryCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new UserAccountDTO
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Profile = reader.GetString(reader.GetOrdinal("Profile"))
                };
            }
            return null;
        }

        public async Task<int?> GetIdWithNameOrEmail(string nameOrEmail)
        {
            object? result = null;
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();
          

            string getIdWithNameQuery = @"SELECT id FROM user WHERE Name = @name";

            using var getIdWithNameQueryCommand = new SqliteCommand(getIdWithNameQuery, connection);
            getIdWithNameQueryCommand.Parameters.AddWithValue("@name", nameOrEmail);

            result = await getIdWithNameQueryCommand.ExecuteScalarAsync();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }

            string getIdWithEmailQuery = @"SELECT id FROM user WHERE Email = @email";

            using var getIdWithEmailQueryCommand = new SqliteCommand(getIdWithEmailQuery, connection);
            getIdWithEmailQueryCommand.Parameters.AddWithValue("@email", nameOrEmail);

            result = await getIdWithEmailQueryCommand.ExecuteScalarAsync();

            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            return null;

        }
    }
}
