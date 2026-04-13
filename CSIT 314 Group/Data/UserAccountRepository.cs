using CSIT_314_Group.DTO;
using CSIT_314_Group.Entity;
using CSIT_314_Group.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace CSIT_314_Group.Data
{
    public class UserAccountRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        public UserAccountRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<CreateUserResultEnum> CreateUser(UserAccountEntity userDetails)
        {
            var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            SqliteTransaction transaction = connection.BeginTransaction();
            try
            {
                string createUserQuery = @"INSERT INTO user ( Name, PhoneNumber, Email, HashedPassword) VALUES ( @Name, @PhoneNumber, @Email, @HashedPassword)";

                await using var createUserQueryCommand = new SqliteCommand(createUserQuery, connection, transaction);
                
                    createUserQueryCommand.Parameters.AddWithValue("@Name", userDetails.Name);
                    createUserQueryCommand.Parameters.AddWithValue("@PhoneNumber", userDetails.PhoneNumber);
                    createUserQueryCommand.Parameters.AddWithValue("@Email", userDetails.Email);
                    createUserQueryCommand.Parameters.AddWithValue("@HashedPassword", userDetails.HashedPassword);

                    int rowsAffected = await createUserQueryCommand.ExecuteNonQueryAsync();

                
                if( rowsAffected != 1)
                {
                    await transaction.RollbackAsync();
                    return CreateUserResultEnum.Failed;
                }
                await transaction.CommitAsync();
                return CreateUserResultEnum.Success;
            }
            catch(SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
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
    }
}
