using CSIT_314_Group.DTO.CategoryDTO;
using CSIT_314_Group.Entity;
using Microsoft.Data.Sqlite;


namespace CSIT_314_Group.Data;

public class CategoryRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

        public CategoryRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<ViewCategoryDTO?> GetById(int? id)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"SELECT * FROM FundraiserCategory WHERE Id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ViewCategoryDTO(
                    reader.GetInt32(reader.GetOrdinal("Id")),
                    reader.GetString(reader.GetOrdinal("FraCategoryName")),
                    reader.GetString(reader.GetOrdinal("Desc"))
                );
            }

            return null;
        }

        public async Task<ViewCategoryDTO?> GetByName(string name)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"SELECT * FROM FundraiserCategory WHERE LOWER(FraCategoryName) = @name";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@name", name.ToLower());

            var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ViewCategoryDTO(
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

        public async Task<List<ViewCategoryDTO>> ViewAllCategories()
        {
            List<ViewCategoryDTO> result = new List<ViewCategoryDTO>();

            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"SELECT * FROM FundraiserCategory";
            using var command = new SqliteCommand(query, connection);

            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new ViewCategoryDTO(
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

        public async Task<List<ViewCategoryDTO>> SearchCategories(string keyword)
        {
            List<ViewCategoryDTO> result = new List<ViewCategoryDTO>();

            using var connection = _dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            string query = @"SELECT * FROM FundraiserCategory WHERE LOWER(FraCategoryName) LIKE @keyword OR LOWER(Desc) LIKE @keyword";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@keyword", $"%{keyword.ToLower()}%");

            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new ViewCategoryDTO(
                    reader.GetInt32(reader.GetOrdinal("Id")),
                    reader.GetString(reader.GetOrdinal("FraCategoryName")),
                    reader.GetString(reader.GetOrdinal("Desc"))
                ));
            }

            return result;
        }
}