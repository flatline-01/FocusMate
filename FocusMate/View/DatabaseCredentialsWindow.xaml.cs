using System.Windows;

namespace FocusMate.View
{
    public partial class DatabaseCredentialsWindow : Window
    {
        public DatabaseCredentialsWindow()
        {
            InitializeComponent();
        }

        private void SaveCredentials(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("dbms_username", UsernameTextBox.Text, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("dbms_password", PasswordTextBox.Password, EnvironmentVariableTarget.Machine);
            Close();
        }
    }
}
