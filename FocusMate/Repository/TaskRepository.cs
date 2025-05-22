using Task = FocusMate.Model.Task;
using Npgsql;

namespace FocusMate.Repository
{
    public class TaskRepository
    {
        private string _tableName = "tasks";
        private NpgsqlConnection _connection;

        public TaskRepository(NpgsqlConnection connection) { 
            _connection = connection;
        }

        public void CreateTask(Task task) {
            var command = new NpgsqlCommand($"INSERT INTO {_tableName} " +
                $"(title, category_id, date, is_done) VALUES" +
                "(@title, @category_id, @date, @is_done)", _connection);
            command.Parameters.AddWithValue("@title", task.Title);
            command.Parameters.AddWithValue("@category_id", task.CategoryId);
            command.Parameters.AddWithValue("@date", task.Date);
            command.Parameters.AddWithValue("@is_done", task.IsDone);
            command.ExecuteNonQuery();
        }

        public void UpdateTask(Task task) {
            var command = new NpgsqlCommand($"UPDATE {_tableName} SET title = @title, " +
                $"category_id = @category_id, date = @date, is_done = @is_done WHERE id = @id", _connection);
            command.Parameters.AddWithValue("@title", task.Title);
            command.Parameters.AddWithValue("@category_id", task.CategoryId);
            command.Parameters.AddWithValue("@date", task.Date);
            command.Parameters.AddWithValue("@is_done", task.IsDone);
            command.Parameters.AddWithValue("@id", task.Id);
            command.ExecuteNonQuery();
        } 

        public void DeleteTask(int id) {
            var command = new NpgsqlCommand($"DELETE FROM {_tableName} WHERE id = @id", 
                _connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public List<Task> GetAllPendingTasks() {
            var tasks = new List<Task>();

            var command = new NpgsqlCommand($"SELECT * FROM {_tableName} WHERE is_done = false", _connection);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read()) { 
                Task task = new Task();
                task.Id = reader.GetInt32(0);
                task.Title = reader.GetString(1);
                task.CategoryId = reader.GetInt32(2);
                task.Date = reader.GetDateTime(3);
                task.IsDone = reader.GetBoolean(4);
                tasks.Add(task);
            }
            reader.Close();
            return tasks; 
        }

        public List<Task> GetAllTasksByCategoryId(int categoryId) {
            var tasks = new List<Task>();

            var command = new NpgsqlCommand($"SELECT * FROM {_tableName} WHERE category_id = @category_id", 
                _connection);
            command.Parameters.AddWithValue(categoryId);
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Task task = new Task();
                task.Id = reader.GetInt32(0);
                task.Title = reader.GetString(1);
                task.CategoryId = reader.GetInt32(2);
                task.Date = reader.GetDateTime(3);
                task.IsDone = reader.GetBoolean(4);
                tasks.Add(task);
            }
            reader.Close();
            return tasks;
        }

        public Task GetTaskById(int id) {
            var command = new NpgsqlCommand($"SELECT * FROM {_tableName} WHERE id = @id", _connection);
            command.Parameters.AddWithValue("@id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            Task task = new Task();

            while (reader.Read()) {
                task.Id = reader.GetInt32(0);
                task.Title = reader.GetString(1);
                task.CategoryId = reader.GetInt32(2);
                task.Date = reader.GetDateTime(3);
                task.IsDone = reader.GetBoolean(4);
            }
            reader.Close();
            return task;
        }
    }
}
