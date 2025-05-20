using FocusMate.Repository;
using System.Windows;
using Npgsql;
using Task = FocusMate.Model.Task;
using System.Windows.Controls;

namespace FocusMate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskRepository _taskRepository;
        CategoryRepository _categoryRepository;
        public MainWindow()
        {
            InitializeComponent();
            
            DatabaseConnector connector = new DatabaseConnector();
            NpgsqlConnection connection = connector.GetConnection();
            _taskRepository = new TaskRepository(connection);
            _categoryRepository = new CategoryRepository(connection);

            LoadTasks();
        }

        private void LoadTasks() {
            List<Task> tasks = _taskRepository.GetAllTasks();
            if (tasks.Count > 0)
            {
                NoTasks.Visibility = Visibility.Collapsed;
            }
            SetCategoryNames(tasks);
            TasksList.ItemsSource = tasks;
        }

        private void SetCategoryNames(List<Task> tasks) {
            foreach (var task in tasks) {
                task.CategoryName = _categoryRepository.GetCategoryById(task.Id).Name;
            }
        }

        private void DataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id" || e.PropertyName == "CategoryId")
                e.Column = null;
        }

        private void AddTaskButtonClick(object sender, RoutedEventArgs e)
        {
            TaskEditorWindow window = new TaskEditorWindow();
            window.Closing += TaskEditorWindowClosing;
            window.ShowDialog();
        }

        private void TaskEditorWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) { 
            TasksList.ItemsSource = _taskRepository.GetAllTasks();
            (sender as Window).Closing -= TaskEditorWindowClosing;
        }

        private void AddCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            CategoryEditorWindow window = new CategoryEditorWindow();
            window.ShowDialog();
        }

        private void StartTimer(object sender, RoutedEventArgs e) { 
            
        }
    }
}