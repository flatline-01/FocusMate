using Task = FocusMate.Model.Task;
using System.Windows.Controls;
using FocusMate.Repository;
using System.Windows;
using System.Windows.Media;

namespace FocusMate.View
{
    public interface IWindow
    {
        public void LoadContent();

        public void CreateColumn(int index, int width, Templates template, DataGrid dataGrid)
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
            dataGrid.Columns.Add(column);
        }

        public void SetCategoryNames(List<Task> tasks, CategoryRepository repository)
        {
            foreach (var task in tasks)
                task.CategoryName = repository.GetCategoryById(task.CategoryId).Name;
        }

        public object GetElement(DependencyObject obj, DataGrid dataGrid)
        {
            DataGridRow row = GetRow(obj);
            int rowIndex = row.GetIndex();
            return dataGrid.Items.GetItemAt(rowIndex);
        }

        DataGridRow GetRow(DependencyObject obj)
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

        DependencyObject FindParent(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent != null)
                return parent;
            else
                return FindParent(parent);
        }

        public enum Templates
        {
            StartButtonTemplate,
            EditButtonTemplate,
            DeleteButtonTemplate,
            CheckboxTemplate
        }
    }
}
