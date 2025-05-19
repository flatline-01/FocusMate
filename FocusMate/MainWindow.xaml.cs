using FocusMate.Repository;
using System.Windows;
using Npgsql;
using Task = FocusMate.Model.Task;

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
            TaskEditorWindow window = new TaskEditorWindow();
            window.ShowDialog();
        }

        private void AddCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            CategoryEditorWindow window = new CategoryEditorWindow();
            window.ShowDialog();
        }
    }
}