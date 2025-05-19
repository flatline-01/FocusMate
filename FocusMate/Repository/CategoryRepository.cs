using FocusMate.Model;
using Npgsql;

namespace FocusMate.Repository
{
    public class CategoryRepository
    {
        private string _tableName = "categories";
        private NpgsqlConnection _connection;

        public CategoryRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public void CreateCategory(Category category)
        {
            var command = new NpgsqlCommand($"INSERT INTO {_tableName} " +
                $"(name) VALUES (@name)", _connection);
            command.Parameters.AddWithValue("@name", category.Name);
            command.ExecuteNonQuery();
        }

        public void UpdateCategory(Category category)
        {
            var command = new NpgsqlCommand($"UPDATE {_tableName} SET name = val1 " +
                $"WHERE id = val2", _connection);
            command.Parameters.AddWithValue("val1", category.Name);
            command.Parameters.AddWithValue("val2", category.Id);
            command.ExecuteNonQuery();
        }

        public void DeleteCategory(int id)
        {
            var command = new NpgsqlCommand($"DELETE FROM {_tableName} WHERE id = val1",
                _connection);
            command.Parameters.AddWithValue("val1", id);
            command.ExecuteNonQuery();
        }

        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();

            var command = new NpgsqlCommand($"SELECT * FROM {_tableName}", _connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Category category = new Category();
                category.Id = reader.GetInt32(0);
                category.Name = reader.GetString(1);
                categories.Add(category);
            }
            reader.Close();
            return categories;
        }

        public Category GetCategoryById(int id)
        {
            var command = new NpgsqlCommand($"SELECT * FROM {_tableName} WHERE id = @id", _connection);
            command.Parameters.AddWithValue("@id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            Category category = new Category();
            while (reader.Read())
            {
                category.Id = reader.GetInt32(0);
                category.Name = reader.GetString(1);
            }
            reader.Close();
            return category;
        }
    }
}
