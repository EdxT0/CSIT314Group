using CSIT_314_Group.DTO.UserProfileDTO;
using CSIT_314_Group.Entity;
using Microsoft.Data.Sqlite;

namespace CSIT_314_Group.Data;

public class UserProfileRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public UserProfileRepository(DbConnectionFactory factory)
    {
        _dbConnectionFactory = factory;
    }


    // Create Profile
    public async Task<bool> CreateUserProfile(UserProfile userProfile)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            string query = @"
                INSERT INTO UserProfile (Id, ProfileName, Description, Status)
                VALUES (@Id, @ProfileName, @Description, @Status)";

            using var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@Id", userProfile.Id);
            command.Parameters.AddWithValue("@ProfileName", userProfile.ProfileName);
            command.Parameters.AddWithValue("@Description", userProfile.Description);
            command.Parameters.AddWithValue("@Status", userProfile.Status);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int?> getIdWithProfileName(string profileName)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string getIdWithProfileNameQuery = @"SELECT Id from UserProfile WHERE ProfileName = @profileName";
        using var getIdWithProfileNameQueryCommand = new SqliteCommand(getIdWithProfileNameQuery, connection);
        getIdWithProfileNameQueryCommand.Parameters.AddWithValue("@profileName", profileName);
        var result = await getIdWithProfileNameQueryCommand.ExecuteScalarAsync();
        if (result == null)
        {
            return null;
        }
        return Convert.ToInt32(result);
    }

    public async Task<string> getProfileNameWithId(int? id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string getProfileNameWithIdQuery = @"SELECT ProfileName from UserProfile WHERE Id = @id";
        using var getProfileNameWithIdQueryCommand = new SqliteCommand(getProfileNameWithIdQuery, connection);
        getProfileNameWithIdQueryCommand.Parameters.AddWithValue("@id", id);
        var result = await getProfileNameWithIdQueryCommand.ExecuteScalarAsync();

        return Convert.ToString(result);
    }


    // View Profile
    public async Task<UserProfile?> GetUserProfile(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT Id, ProfileName, Description, Status
            FROM UserProfile
            WHERE Id = @Id";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new UserProfile
            {
                Id = Convert.ToInt32(reader["Id"]),
                ProfileName = reader["ProfileName"].ToString() ?? "",
                Description = reader["Description"].ToString() ?? "",
                Status = reader["Status"].ToString() ?? ""
            };
        }

        return null;
    }


    // Update 
    public async Task<bool> UpdateUserProfile(UpdateUserProfileDTO request)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            string query = @"
            UPDATE UserProfile
            SET ProfileName = @ProfileName,
                Description = @Description
            WHERE Id = @Id";

            using var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ProfileName", request.ProfileName.Trim().ToLower());
            command.Parameters.AddWithValue("@Description", request.Description.Trim().ToLower());
            command.Parameters.AddWithValue("@Id", request.Id);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    // Search User Profile
    public async Task<List<UserProfile>> SearchUserProfile(string keyword)
    {
        var profiles = new List<UserProfile>();

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT Id, ProfileName, Description, Status
            FROM UserProfile
            WHERE lower(ProfileName) LIKE @Keyword
               OR lower(Description) LIKE @Keyword
               OR lower(Status) LIKE @Keyword";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@Keyword", "%" + keyword.Trim().ToLower() + "%");

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            profiles.Add(new UserProfile
            {
                Id = Convert.ToInt32(reader["Id"]),
                ProfileName = reader["ProfileName"].ToString() ?? "",
                Description = reader["Description"].ToString() ?? "",
                Status = reader["Status"].ToString() ?? ""
            });
        }

        return profiles;
    }


    // Suspend User Profile
    public async Task<bool> SuspendUserProfile(int id, string status)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            string query = @"
            UPDATE UserProfile
            SET Status = @Status
            WHERE Id = @Id";

            using var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    // Check if ProfileName is Suspended
    public async Task<bool> IsProfileSuspended(string profileName)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT Status
            FROM UserProfile
            WHERE ProfileName = @ProfileName";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@ProfileName", profileName);

        var result = await command.ExecuteScalarAsync();

        if (result == null)
            return false;

        return result.ToString()?.ToLower() == "suspended";
    }

    public async Task<bool> IsProfileSuspended(int? profileId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT Status
            FROM UserProfile
            WHERE Id = @profileId";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@profileId", profileId);

        var result = await command.ExecuteScalarAsync();

        if (result == null)
            return false;

        return Convert.ToBoolean(result);
    }

    // Get All Profiles
    public async Task<List<UserProfile>> GetAllUserProfiles()
    {
        var profiles = new List<UserProfile>();

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
            SELECT Id, ProfileName, Description, Status
            FROM UserProfile";

        using var command = new SqliteCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            profiles.Add(new UserProfile
            {
                Id = Convert.ToInt32(reader["Id"]),
                ProfileName = reader["ProfileName"].ToString() ?? "",
                Description = reader["Description"].ToString() ?? "",
                Status = reader["Status"].ToString() ?? ""
            });
        }

        return profiles;
    }
}
