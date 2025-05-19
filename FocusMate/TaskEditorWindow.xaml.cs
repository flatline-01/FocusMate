using FocusMate.Repository;
using Npgsql;
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

        public TaskEditorWindow()
        {
            InitializeComponent();

            DatabaseConnector connector = new DatabaseConnector();
            NpgsqlConnection connection = connector.GetConnection();
            _categoryRepository = new CategoryRepository(connection);
            LoadCategoriesDropBox();
        }

        private void LoadCategoriesDropBox() {
            List<Category> categories = _categoryRepository.GetAllCategories();
            if (categories.Count != 0) 
                CategoriesComboBox.ItemsSource = categories;
            else CategoriesComboBox.SelectedItem = "Default";
        }
    }
}
