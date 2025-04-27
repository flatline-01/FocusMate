using FocusMate.Repository;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace FocusMate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // initialize TasksList
            DatabaseConnector connector = new DatabaseConnector();
            NpgsqlConnection connection = connector.GetConnection();
            TaskRepository taskRepository = new TaskRepository(connection);
            List<Task> tasks = taskRepository.GetAllTasks();

            if (tasks.Count > 0)
            {
                NoTasks.Visibility = Visibility.Collapsed;
            }

            TasksList.ItemsSource = tasks;
        }

        private void AddTaskButtonClick(object sender, RoutedEventArgs e)
        {
            EditorWindow editor = new EditorWindow();
            editor.Show();
        }
    }
}