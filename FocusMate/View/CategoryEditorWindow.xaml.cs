using FocusMate.Model;
using FocusMate.Repository;
using Npgsql;
using System.Windows;

namespace FocusMate
{
    /// <summary>
    /// Interaction logic for CategoryEditorWindow.xaml
    /// </summary>
    public partial class CategoryEditorWindow : Window
    {
        CategoryRepository _categoryRepository;
        public CategoryEditorWindow()
        {
            InitializeComponent();
            DatabaseConnector databaseConnector = new DatabaseConnector();
            NpgsqlConnection connections = databaseConnector.GetConnection();
            _categoryRepository = new CategoryRepository(connections);
            LoadCategories();
        }

        private void LoadCategories()
        {
            List<Category> categories = _categoryRepository.GetAllCategories();
            if (categories.Count != 0) {
                CategoriesListBox.ItemsSource = categories;
                NoCategoriesTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void AddNewCategoryButtobClick(object sender, RoutedEventArgs e)
        {
            Category category = new Category();
            category.Name = CategoryNameTextBox.Text;
            _categoryRepository.CreateCategory(category);
            CategoryNameTextBox.Text = "";
            MessageBox.Show($"Category \"{category.Name}\" added successfully.");
            CategoriesListBox.ItemsSource = _categoryRepository.GetAllCategories();
        }
    }
}
