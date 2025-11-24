using FocusMate.Repository;
using Npgsql;
using Task = FocusMate.Model.Task;
using FocusMate.Model;
using System.Windows;
using System.Windows.Controls;

namespace FocusMate
{
    public partial class TaskEditorWindow : Window
    {
        private CategoryRepository _categoryRepository;
        private TaskRepository _taskRepository;
        private Task _task;

        public TaskEditorWindow(Task task)
        {
            _task = task;

            InitializeComponent();

            DatabaseConnector connector = new DatabaseConnector();
            NpgsqlConnection connection = connector.GetConnection();
            _categoryRepository = new CategoryRepository(connection);
            _taskRepository = new TaskRepository(connection);
            LoadCategoriesDropBox();

            if (_task != null) {
                TaskTitleTextBox.Text = task.Title;
                DatePicker.SelectedDate = task.Date;

                for (int i = 0; i < CategoriesComboBox.Items.Count; i++) {
                    Category category = (Category) CategoriesComboBox.Items.GetItemAt(i);
                    if (category.Id == task.CategoryId)
                        CategoriesComboBox.SelectedIndex = task.CategoryId - 1;
                }
            }
        }

        private void LoadCategoriesDropBox() {
            List<Category> categories = _categoryRepository.GetAllCategories();
            if (categories.Count != 0) 
                CategoriesComboBox.ItemsSource = categories;
        }

        private void CheckIfDateInThePast(object sender, RoutedEventArgs e) {
            DatePicker datePicker = (DatePicker) sender;

            if (datePicker.SelectedDate < DateTime.Today) {
                MessageBox.Show("The date shouldn't be in the past.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                datePicker.SelectedDate = DateTime.Today;
            }
        }

        private void SaveTaskButtonClick(object sender, RoutedEventArgs e)
        {
            if (CategoriesComboBox.SelectedItem is null || TaskTitleTextBox.Text is null || DatePicker.SelectedDate is null)
                MessageBox.Show("You must specify values for each field.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            else
            {
                int categoryId = _categoryRepository.GetCategoryIdByName(CategoriesComboBox.SelectedItem.ToString());
                string message = "";
                bool wasNull = false;

                if (_task == null)
                {
                    wasNull = true;
                    _task = new Task();
                }

                _task.Title = TaskTitleTextBox.Text;
                _task.CategoryId = categoryId;
                _task.Date = DatePicker.SelectedDate.Value;
                _task.IsDone = false;

                if (wasNull)
                {
                    _taskRepository.CreateTask(_task);
                    message = $"Task \"{_task.Title}\" created successfully.";
                }
                else
                {
                    _taskRepository.UpdateTask(_task);
                    message = $"Task \"{_task.Title}\" updated successfully.";
                }

                _task = null;
                MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
            }
        }

        private void ClearFields() {
            TaskTitleTextBox.Text = string.Empty;
            DatePicker.SelectedDate = null;
            CategoriesComboBox.SelectedItem = null;
        }
    }
}
