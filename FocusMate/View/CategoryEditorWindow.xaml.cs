using FocusMate.Model;
using FocusMate.Repository;
using FocusMate.View;
using Npgsql;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Templates = FocusMate.View.IWindow.Templates;

namespace FocusMate
{
    public partial class CategoryEditorWindow : Window, IWindow
    {
        private CategoryRepository _categoryRepository;
        private Category _category;

        public CategoryEditorWindow()
        {
            _category = null;

            InitializeComponent();

            DatabaseConnector databaseConnector = new DatabaseConnector();
            NpgsqlConnection connections = databaseConnector.GetConnection();
            _categoryRepository = new CategoryRepository(connections);

            LoadContent();
        }

        public void LoadContent()
        {
            List<Category> categories = _categoryRepository.GetAllCategories();
            if (categories.Count > 0) 
                NoCategoriesTextBlock.Visibility = Visibility.Collapsed;

            if (CategoriesList.Columns.Count != 3) {
                ((IWindow)this).CreateColumn(1, 30, Templates.EditButtonTemplate, CategoriesList);
                ((IWindow)this).CreateColumn(2, 30, Templates.DeleteButtonTemplate, CategoriesList);
            }

            CategoriesList.ItemsSource = categories;
        }

        private void AddNewCategoryButtobClick(object sender, RoutedEventArgs e)
        {
            bool wasNull = false;
            if (_category == null) { 
                _category = new Category();
                wasNull = true;
            }
            _category.Name = CategoryNameTextBox.Text;

            if (wasNull) 
                _categoryRepository.CreateCategory(_category);
            else
                _categoryRepository.UpdateCategory(_category);
            CategoryNameTextBox.Text = "";
            MessageBox.Show($"Category \"{_category.Name}\" saved successfully.");
            LoadContent();
            _category = null;
        }

        private void DataGridAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            if (e.Column is not null)
            {
                e.Column.CanUserResize = false;
                e.Column.IsReadOnly = true;
                e.Column.CanUserReorder = false;
            }

            switch (e.PropertyName)
            {
                case "Id":
                    e.Column = null;
                    break;
                case "Name":
                    e.Column.Width = 185;
                    e.Column.DisplayIndex = 0;
                    break;
            }
        }

        private void EditCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            Category category = (Category) ((IWindow)this).GetElement((Button) sender, CategoriesList);
            CategoryNameTextBox.Text = category.Name;
            _category = category;
        }

        private DependencyObject FindParent(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent != null)
                return parent;
            else
                return FindParent(parent);
        }

        private void DeleteCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            Category category = (Category)((IWindow)this).GetElement((Button)sender, CategoriesList);
            _categoryRepository.DeleteCategory(category.Id);
            LoadContent();   
        }
    }
}
