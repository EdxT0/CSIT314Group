using Microsoft.Data.Sqlite;


namespace CSIT_314_Group.Data;

public class Category
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Category()
    {
    }
    public Category(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public Category(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    public Category(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Category?> GetById(int? id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"SELECT * FROM FundraiserCategory WHERE Id = @id";
        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Category(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("FraCategoryName")),
                reader.GetString(reader.GetOrdinal("Desc"))
            );
        }

        return null;
    }

    public async Task<Category?> GetByName(string name)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"SELECT * FROM FundraiserCategory WHERE LOWER(FraCategoryName) = @name";
        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@name", name.ToLower());

        var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Category(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("FraCategoryName")),
                reader.GetString(reader.GetOrdinal("Desc"))
            );
        }

        return null;
    }

    public async Task<bool> CreateCategory(Category category)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
                INSERT INTO FundraiserCategory (FraCategoryName, Desc)
                VALUES (@name, @description)
            ";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@description", category.Description);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected == 1;
    }

    public async Task<List<Category>> ViewAllCategories()
    {
        List<Category> result = new List<Category>();

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"SELECT * FROM FundraiserCategory";
        using var command = new SqliteCommand(query, connection);

        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Category(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("FraCategoryName")),
                reader.GetString(reader.GetOrdinal("Desc"))
            ));
        }

        return result;
    }

    public async Task<bool> UpdateCategory(Category category)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"
                UPDATE FundraiserCategory
                SET FraCategoryName = @name,
                    Desc = @description
                WHERE Id = @id
            ";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@id", category.Id);
        command.Parameters.AddWithValue("@name", category.Name);
        command.Parameters.AddWithValue("@description", category.Description);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected == 1;
    }

    public async Task<bool> DeleteCategory(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"DELETE FROM FundraiserCategory WHERE Id = @id";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected == 1;
    }

    public async Task<List<Category>> SearchCategories(string keyword)
    {
        List<Category> result = new List<Category>();

        using var connection = _dbConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string query = @"SELECT * FROM FundraiserCategory WHERE LOWER(FraCategoryName) LIKE @keyword OR LOWER(Desc) LIKE @keyword";

        using var command = new SqliteCommand(query, connection);
        command.Parameters.AddWithValue("@keyword", $"%{keyword.ToLower()}%");

        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            result.Add(new Category(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("FraCategoryName")),
                reader.GetString(reader.GetOrdinal("Desc"))
            ));
        }

        return result;
    }
}