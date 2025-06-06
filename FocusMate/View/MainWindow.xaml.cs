using FocusMate.Repository;
using System.Windows;
using Npgsql;
using Task = FocusMate.Model.Task;
using System.Windows.Controls;
using System.ComponentModel;
using FocusMate.View;
using Templates = FocusMate.View.IWindow.Templates;

namespace FocusMate
{
    public partial class MainWindow : Window, IWindow
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

            LoadContent();
            CreateReminder();
        }

        public void LoadContent() {
            List<Task> tasks = _taskRepository.GetAllPendingTasks();
            if (tasks.Count > 0)
                NoTasks.Visibility = Visibility.Collapsed;

            ((IWindow)this).SetCategoryNames(tasks, _categoryRepository);

            if (TasksList.Columns.Count != 9)
            {
                ((IWindow)this).CreateColumn(5, 20, Templates.CheckboxTemplate, TasksList);
                ((IWindow)this).CreateColumn(6, 30, Templates.StartButtonTemplate, TasksList);
                ((IWindow)this).CreateColumn(7, 30, Templates.EditButtonTemplate, TasksList);
                ((IWindow)this).CreateColumn(8, 30, Templates.DeleteButtonTemplate, TasksList);
            }
            
            TasksList.ItemsSource = tasks;
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
                    e.Column.Width = 290;
                    e.Column.DisplayIndex = 0;
                    break;
                case "CategoryName":
                    e.Column.Header = "Category";
                    e.Column.Width = 110;
                    e.Column.DisplayIndex = 1;
                    break;
                case "IsDone":
                    e.Column = null; 
                    break;
                case "Date":
                    e.Column.Width = 70;
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

        private void TaskEditorWindowClosing(object sender, CancelEventArgs e) {
            LoadContent();
            (sender as Window).Closing -= TaskEditorWindowClosing;
        }

        private void AddCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            CategoryEditorWindow window = new CategoryEditorWindow();
            window.Closing += CategoryEditorWindowClosing;
            window.ShowDialog();
        }

        private void CategoryEditorWindowClosing(object sender, CancelEventArgs e)
        {
            LoadContent();
            (sender as Window).Closing -= CategoryEditorWindowClosing;
        }


        private void ViewProgressButtonClick(object sender, RoutedEventArgs e)
        {
            ProgressWindow window = new ProgressWindow();
            window.Show();
        }

        private void StartTaskButtonClick(object sender, RoutedEventArgs e) {
            TimerWindow window = new TimerWindow();
            window.ShowDialog();
        }

        private void EditTaskButtonClick(object sender, RoutedEventArgs e) {
            Task task = (Task) ((IWindow)this).GetElement((Button) sender, TasksList);
            var window = new TaskEditorWindow(task);
            window.Closing += TaskEditorWindowClosing;
            window.ShowDialog();
        }

        private void DeleteTaskButtonClick(object sender, RoutedEventArgs e) {
            Task task = (Task)((IWindow)this).GetElement((Button) sender, TasksList);
            _taskRepository.DeleteTask(task.Id);
            LoadContent();
        }

        private async void TaskCompletingHandler(object sender, EventArgs e) {
            CheckBox cb = (CheckBox) sender;
            Task task = (Task) ((IWindow)this).GetElement(cb, TasksList);
            task.IsDone = true;
            _taskRepository.UpdateTask(task);

            await System.Threading.Tasks.Task.Delay(500);

            IEditableCollectionView items = TasksList.Items;
            if (items.CanRemove && cb.IsChecked.HasValue)
                items.Remove(task);
        }

        private void CreateReminder() {
            int unresolvedTasksNumber = _taskRepository.CountPendingTasksForToday();
            Reminder r = new Reminder();
            r.IsInfinite = true;
            r.Delay = 240;
            r.Text = $"You have {unresolvedTasksNumber} tasks to accomplish for today.";
            r.DisplayMessage();
        }
    }
}