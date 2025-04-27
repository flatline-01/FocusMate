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
                "(val1, val2, val3, val4)", _connection);
            command.Parameters.AddWithValue("val1", task.Title);
            command.Parameters.AddWithValue("val2", task.CategoryId);
            command.Parameters.AddWithValue("val3", task.Date);
            command.Parameters.AddWithValue("val4", task.IsDone);
            command.ExecuteNonQuery();
        }

        public void UpdateTask(Task task) {
            var command = new NpgsqlCommand($"UPDATE {_tableName} SET title = val1, " +
                $"category_id = val2, date = val3, is_done = val4 WHERE id = val5", _connection);
            command.Parameters.AddWithValue("val1", task.Title);
            command.Parameters.AddWithValue("val2", task.CategoryId);
            command.Parameters.AddWithValue("val3", task.Date);
            command.Parameters.AddWithValue("val4", task.IsDone);
            command.Parameters.AddWithValue("val5", task.Id);
            command.ExecuteNonQuery();
        } 

        public void DeleteTask(int id) {
            var command = new NpgsqlCommand($"DELETE FROM {_tableName} WHERE id = val1", 
                _connection);
            command.Parameters.AddWithValue("val1", id);
            command.ExecuteNonQuery();
        }

        public List<Task> GetAllTasks() {
            var tasks = new List<Task>();

            var command = new NpgsqlCommand($"SELECT * FROM {_tableName}", _connection);
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

            return tasks; 
        }

        public List<Task> GetAllTasksByCategoryId(int categoryId) {
            var tasks = new List<Task>();

            var command = new NpgsqlCommand($"SELECT * FROM {_tableName} WHERE category_id = val1", 
                _connection);
            command.Parameters.Add(categoryId);
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

            return tasks;
        }

        public Task GetTaskById(int id) {
            var command = new NpgsqlCommand($"SELECT * FROM {_tableName} WHERE id = val1", _connection);
            command.Parameters.AddWithValue("val1", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            Task task = new Task();

            while (reader.Read()) {
                task.Id = reader.GetInt32(0);
                task.Title = reader.GetString(1);
                task.CategoryId = reader.GetInt32(2);
                task.Date = reader.GetDateTime(3);
                task.IsDone = reader.GetBoolean(4);
            }

            return task;
        }
    }
}
