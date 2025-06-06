using FocusMate.Repository;
using System.Windows;
using Npgsql;
using Task = FocusMate.Model.Task;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using FocusMate.View;
using static FocusMate.MainWindow;
using System;

namespace FocusMate
{
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
            CreateReminder();
        }

        private void LoadTasks() {
            List<Task> tasks = _taskRepository.GetAllPendingTasks();
            if (tasks.Count > 0)
                NoTasks.Visibility = Visibility.Collapsed;

            SetCategoryNames(tasks);

            if (TasksList.Columns.Count != 9)
            {
                CreateColumn(5, 20, Templates.CheckboxTemplate);
                CreateColumn(6, 30, Templates.StartButtonTemplate);
                CreateColumn(7, 30, Templates.EditButtonTemplate);
                CreateColumn(8, 30, Templates.DeleteButtonTemplate);
            }
            
            TasksList.ItemsSource = tasks;
        }

        private void CreateColumn(int index, int width, Templates template) {
            DataGridTemplateColumn column = new DataGridTemplateColumn
            {
                CanUserReorder = false,
                Width = width,
                CanUserResize = false,
                CanUserSort = false,
                Header = string.Empty,
                DisplayIndex = index,
                CellTemplateSelector = new CustomTemplateSelector(template)
            };
            TasksList.Columns.Add(column);
        }

        private void SetCategoryNames(List<Task> tasks) {
            foreach (var task in tasks) 
                task.CategoryName = _categoryRepository.GetCategoryById(task.CategoryId).Name;
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
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                case "Title":
                    e.Column.Width = 300;
                    e.Column.DisplayIndex = 0;
                    break;
                case "CategoryName":
                    e.Column.Header = "Category";
                    e.Column.Width = 70;
                    e.Column.DisplayIndex = 1;
                    break;
                case "IsDone":
                    e.Column = null; 
                    break;
                case "Date":
                    e.Column.Width = 55;
                    e.Column.DisplayIndex = 2;
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yy";
                    break;
            }
        }

        private void AddTaskButtonClick(object sender, RoutedEventArgs e)
        {
            TaskEditorWindow window = new TaskEditorWindow(null);
            window.Closing += TaskEditorWindowClosing;
            window.ShowDialog();
        }

        private void TaskEditorWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            LoadTasks();
            (sender as Window).Closing -= TaskEditorWindowClosing;
        }

        private void AddCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            CategoryEditorWindow window = new CategoryEditorWindow();
            window.ShowDialog();
        }

        private void StartTaskButtonClick(object sender, RoutedEventArgs e) {
            TimerWindow window = new TimerWindow();
            window.ShowDialog();
        }

        private void EditTaskButtonClick(object sender, RoutedEventArgs e) {
            Button b = (Button) sender;
            DataGridRow row = GetRow(b);
            int rowIndex = row.GetIndex();
            Task task = (Task) TasksList.Items.GetItemAt(rowIndex);
            var window = new TaskEditorWindow(task);
            window.Closing += TaskEditorWindowClosing;
            window.ShowDialog();
        }

        private void DeleteTaskButtonClick(object sender, RoutedEventArgs e) { }

        private void TaskCompletingHandler(object sender, EventArgs e) {
            CheckBox cb = (CheckBox) sender;
            DataGridRow row = GetRow(cb);
            int rowIndex = row.GetIndex();
            Task task = (Task) TasksList.Items.GetItemAt(rowIndex);
            task.IsDone = true;
            _taskRepository.UpdateTask(task);

            IEditableCollectionView items = TasksList.Items; 
            if (items.CanRemove)
                items.Remove(task);
        }

        private DataGridRow GetRow(DependencyObject obj) {
            DependencyObject child = obj;
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(child);

                if (child is DataGridRow)
                    return child as DataGridRow;
                else
                    child = parent;
            }
        }

        private DependencyObject FindParent(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent != null)
                return parent;
            else
                return FindParent(parent);
        }

        private void CreateReminder() {
            int unresolvedTasksNumber = _taskRepository.CountPendingTasksForToday();
            Reminder r = new Reminder();
            r.IsInfinite = true;
            r.Delay = 240;
            r.Text = $"You have {unresolvedTasksNumber} tasks to accomplish for today.";
            r.DisplayMessage();
        }

        private void ViewProgressButtonClick(object sender, RoutedEventArgs e) { 
            ProgressWindow window = new ProgressWindow();
            window.Show();  
        }

        public enum Templates { 
            StartButtonTemplate,
            EditButtonTemplate,
            DeleteButtonTemplate,
            CheckboxTemplate
        }
    }
}