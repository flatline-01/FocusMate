using FocusMate.Repository;
using Npgsql;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Task = FocusMate.Model.Task;

namespace FocusMate.View
{
    public partial class ProgressWindow : Window, IWindow
    {
        TaskRepository _taskRepository;
        CategoryRepository _categoryRepository;
        public ProgressWindow()
        {
            InitializeComponent();

            DatabaseConnector connector = new DatabaseConnector();
            NpgsqlConnection connection = connector.GetConnection();
            _taskRepository = new TaskRepository(connection);
            _categoryRepository = new CategoryRepository(connection);

            LoadContent();
            CalculateStatsForThisWeek();
            CalculateStatsForAllTime();
        }

        private void ChangeRowsBackground(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;
            Task task = (Task) e.Row.Item;

            if (row != null)
            {
                if (task.IsDone) 
                    row.Background = Brushes.LightGreen;
                else
                    row.Background= Brushes.IndianRed;
            }                   
        }

        public void LoadContent()
        {
            List<Task> tasks = _taskRepository.GetAllPastTasks();

            if (tasks.Count > 0)
                NoTasks.Visibility = Visibility.Collapsed;

            ((IWindow)this).SetCategoryNames(tasks, _categoryRepository);
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
                case "IsDone":
                    e.Column = null;
                    break;
                case "Title":
                    e.Column.Width = 375;
                    e.Column.DisplayIndex = 0;
                    break;
                case "CategoryName":
                    e.Column.Header = "Category";
                    e.Column.Width = 120;
                    e.Column.DisplayIndex = 1;
                    break;
                case "Date":
                    e.Column.Width = 70;
                    e.Column.DisplayIndex = 2;
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yy";
                    break;
            }
        }

        private void CalculateStatsForThisWeek() {
            DateTime today = DateTime.Today;
            DayOfWeek weekStart = DayOfWeek.Monday;
            DayOfWeek weekEnd = DayOfWeek.Sunday;

            while (today.DayOfWeek != weekStart)
                today = today.AddDays(-1);

            string thisWeekStart = today.ToString("yyyy-MM-dd");

            while (today.DayOfWeek != weekEnd)
                today = today.AddDays(1);

            string thisWeekEnd = today.ToString("yyyy-MM-dd");

            SolvedTasksThisWeek.Text = $"{_taskRepository.CountNumberOfSovedTasksForPeriod(thisWeekStart, thisWeekEnd)}";
            UnsolvedTasksThisWeek.Text = $"{_taskRepository.CountNumberOfUnsolvedTasksForPeriond(thisWeekStart, thisWeekEnd)}";

        }

        private void CalculateStatsForAllTime()
        {
            SolvedTasksAllTime.Text = $"{_taskRepository.CountNumberOfSovedTasksAllTime()}";
            UnsolvedTasksAllTime.Text = $"{_taskRepository.CountNumberOfUnsovedTasksAllTime()}";
        }
    }
}
