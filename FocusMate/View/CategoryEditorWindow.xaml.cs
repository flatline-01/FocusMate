using FocusMate.Model;
using FocusMate.Repository;
using Npgsql;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static FocusMate.MainWindow;

namespace FocusMate
{
    public partial class CategoryEditorWindow : Window
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

            LoadCategories();
        }

        private void LoadCategories()
        {
            List<Category> categories = _categoryRepository.GetAllCategories();
            if (categories.Count > 0) 
                NoCategoriesTextBlock.Visibility = Visibility.Collapsed;

            if (CategoriesList.Columns.Count != 3) {
                CreateColumn(1, 30, Templates.EditButtonTemplate);
                CreateColumn(2, 30, Templates.DeleteButtonTemplate);
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
            LoadCategories();
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

        private void CreateColumn(int index, int width, Templates template)
        {
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
            CategoriesList.Columns.Add(column);
        }

        private void EditCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            Category category = GetCategory((Button) sender);
            CategoryNameTextBox.Text = category.Name;
            _category = category;
        }

        private Category GetCategory(DependencyObject obj)
        {
            DataGridRow row = GetRow(obj);
            int rowIndex = row.GetIndex();
            return (Category) CategoriesList.Items.GetItemAt(rowIndex);
        }

        private DataGridRow GetRow(DependencyObject obj)
        {
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

        private void DeleteCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            Category category = GetCategory((Button) sender);
            _categoryRepository.DeleteCategory(category.Id);
            LoadCategories();   
        }
    }
}
