using FocusMate.Repository;
using Npgsql;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using Task = FocusMate.Model.Task;

namespace FocusMate.View
{
    /// <summary>
    /// Interaction logic for Progress.xaml
    /// </summary>
    public partial class ProgressWindow : Window
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

            LoadTasks();
            CalculateStatsForLastWeek();
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

        private void LoadTasks()
        {
            List<Task> tasks = _taskRepository.GetAllPastTasks();
            if (tasks.Count > 0)
            {
                NoTasks.Visibility = Visibility.Collapsed;
            }
            SetCategoryNames(tasks);

            TasksList.ItemsSource = tasks;
        }

        private void SetCategoryNames(List<Task> tasks)
        {
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
                case "IsDone":
                    e.Column = null;
                    break;
                case "Title":
                    e.Column.Width = 350;
                    e.Column.DisplayIndex = 0;
                    break;
                case "CategoryName":
                    e.Column.Width = 100;
                    e.Column.DisplayIndex = 1;
                    break;
                case "Date":
                    e.Column.Width = 65;
                    e.Column.DisplayIndex = 2;
                    (e.Column as DataGridTextColumn).Binding.StringFormat = "dd.MM.yy";
                    break;
            }
        }

        private void CalculateStatsForLastWeek() { 
            DateTime today = DateTime.Today;
            DayOfWeek weekStart = DayOfWeek.Monday;

            while (today.DayOfWeek != weekStart) 
                today = today.AddDays(-1);

            string previousWeekStart = today.AddDays(-7).ToString("yyyy-MM-dd");
            string previousWeekEnd = today.AddDays(-1).ToString("yyyy-MM-dd");

            SolvedTasksLastWeek.Text = $"{_taskRepository.CountNumberOfSovedTasksLastWeek(
                previousWeekStart, previousWeekEnd)}";

            UnsolvedTasksLastWeek.Text = $"{_taskRepository.CountNumberOfUnsolvedTasksLastWeek(
                previousWeekStart, previousWeekEnd)}";
        }

        private void CalculateStatsForAllTime()
        {
            SolvedTasksAllTime.Text = $"{_taskRepository.CountNumberOfSovedTasksAllTime()}";

            UnsolvedTasksAllTime.Text = $"{_taskRepository.CountNumberOfUnsovedTasksAllTime()}";
        }
    }
}
