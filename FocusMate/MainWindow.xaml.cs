using FocusMate.Repository;
using System.Windows;
using Npgsql;
using Task = FocusMate.Model.Task;
using System.Windows.Controls;
using System.Reflection.PortableExecutable;

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
            CreateButtonColumn();
            CreateCheckBoxColumn();
            TasksList.ItemsSource = tasks;
       
        }

        private void CreateButtonColumn()
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn
            {
                CanUserReorder = false,
                Width = 85,
                CanUserResize = false,
                CanUserSort = false,
                Header = string.Empty,
                DisplayIndex = 4,
                CellTemplateSelector = new CustomTemplateSelector(Templates.ButtonTemplate)
            };
            TasksList.Columns.Add(column);
        }

        private void CreateCheckBoxColumn()
        {
            DataGridTemplateColumn column = new DataGridTemplateColumn { 
                CanUserReorder = false, 
                Width = 20, 
                IsReadOnly = false, 
                CanUserResize = false, 
                Header = string.Empty,
                DisplayIndex = 3,
                CellTemplateSelector = new CustomTemplateSelector(Templates.CheckboxTemplate)
            };
            TasksList.Columns.Add(column);
        }

        private void SetCategoryNames(List<Task> tasks) {
            foreach (var task in tasks) {
                task.CategoryName = _categoryRepository.GetCategoryById(task.Id).Name;
            }
        }

        private void DataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column is not null)
            {
                e.Column.CanUserResize = false;
                e.Column.IsReadOnly = true;
                e.Column.CanUserReorder = false;
            }

            switch (e.PropertyName)
            {
                case "Id":
                case "CategoryId":
                    e.Column = null;
                    break;
                case "Title":
                    e.Column.Width = 250;
                    break;
                case "CategoryName":
                    e.Column.Width = 100;
                    break;
                case "IsDone":
                    e.Column.Header = string.Empty;
                    e.Column.Width = 20;
                    e.Column.IsReadOnly = false;
                    e.Column = null; 
                    break;
                case "Date":
                    e.Column.Width = 60;
                    break;
            }
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

        private void StartTaskButtonClick(object sender, RoutedEventArgs e) {
            MessageBox.Show("something");
        }

        private void TaskCompletingHandler(object sender, EventArgs e) {
            MessageBox.Show("Completed");
        }

        public enum Templates { 
            ButtonTemplate,
            CheckboxTemplate
        }
    }
}