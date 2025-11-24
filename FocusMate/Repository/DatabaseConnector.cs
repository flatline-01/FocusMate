using System.ComponentModel;
using System.Windows;
using FocusMate.View;
using Npgsql;

namespace FocusMate.Repository
{
    public class DatabaseConnector
    {
        private static string _username;
        private static string _password;
        private string _connectionString;

        public DatabaseConnector() {
            SetVariables();
            if (_username == null || _password == null) { 
                AskForCredentials();
            }
        }

        private void AskForCredentials() {
            var window = new DatabaseCredentialsWindow();
            window.Closing += DatabaseCredentialsWindowClosing;
            window.ShowDialog();
        }

        private void DatabaseCredentialsWindowClosing(object sender, CancelEventArgs e)
        {
            SetVariables();
            (sender as Window).Closing -= DatabaseCredentialsWindowClosing;
        }

        private void SetVariables() {
            _username = Environment.GetEnvironmentVariable("dbms_username", EnvironmentVariableTarget.Machine);
            _password = Environment.GetEnvironmentVariable("dbms_password", EnvironmentVariableTarget.Machine);
            _connectionString =
                $"Host=localhost;Port=5432;Database=focus_mate;Username={_username};Password={_password}";
        }

        public NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            try
            {
                connection.Open();
            }
            catch (Exception ex) { 
                MessageBox.Show(ex.Message);
                AskForCredentials();
            }
            return connection;
        }
    }
}
