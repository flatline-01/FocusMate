using System.Windows;
using Npgsql;

namespace FocusMate.Repository
{
    public class DatabaseConnector
    {
        private static string _username = Environment.GetEnvironmentVariable("dbms_username");
        private static string _password = Environment.GetEnvironmentVariable("dbms_password");
        private string _connectionString = 
            $"Host=localhost;Port=5432;Database=focus_mate;Username={_username};Password={_password}";

        public NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.Message);    
            }
            return connection;
        }
    }
}
