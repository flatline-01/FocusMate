using FocusMate.Repository;
using Npgsql;
using Task = FocusMate.Model.Task;
using FocusMate.Model;
using System.Windows;

namespace FocusMate
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TaskEditorWindow : Window
    {
        private CategoryRepository _categoryRepository;
        private TaskRepository _taskRepository;

        public TaskEditorWindow()
        {
            InitializeComponent();

            DatabaseConnector connector = new DatabaseConnector();
            NpgsqlConnection connection = connector.GetConnection();
            _categoryRepository = new CategoryRepository(connection);
            _taskRepository = new TaskRepository(connection);
            LoadCategoriesDropBox();
        }

        private void LoadCategoriesDropBox() {
            List<Category> categories = _categoryRepository.GetAllCategories();
            if (categories.Count != 0) 
                CategoriesComboBox.ItemsSource = categories;
            else CategoriesComboBox.SelectedItem = "Default";
        }

        private void SaveTaskButtonClick(object sender, RoutedEventArgs e)
        {
            int categoryId = _categoryRepository.GetCategoryIdByName(CategoriesComboBox.SelectedItem.ToString());

            Task task = new Task();
            task.Title = TaskTitleTextBox.Text;
            task.CategoryId = categoryId;
            task.Date = DatePicker.SelectedDate.Value;
            task.IsDone = false;
            _taskRepository.CreateTask(task);
            MessageBox.Show($"Task \"{task.Title}\" created successfully.");
            ClearFields();
        }

        private void ClearFields() {
            TaskTitleTextBox.Text = string.Empty;
            DatePicker.SelectedDate = null;
            CategoriesComboBox.SelectedItem = null;
        }
    }
}
