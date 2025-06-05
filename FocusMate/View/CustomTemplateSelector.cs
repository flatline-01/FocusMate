using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Templates = FocusMate.MainWindow.Templates;

namespace FocusMate
{
    public class CustomTemplateSelector : DataTemplateSelector
    {
        private Templates _template;
        public CustomTemplateSelector(Templates template) {
            _template = template;
        }

        public override DataTemplate SelectTemplate(object inItem, DependencyObject inContainer)
        {
            DataRowView row = inItem as DataRowView;
            Window w = GetWindow(inContainer);
            DataTemplate template = null;

            switch (_template)
            {
                case Templates.ButtonTemplate:
                    template = (DataTemplate) w.FindResource("StartTaskButtonTemplate");
                    break;
                case Templates.CheckboxTemplate:
                    template = (DataTemplate) w.FindResource("CompleteTaskCheckBoxTemplate");
                    break;
            }
            return template;
        }

        private Window GetWindow(DependencyObject obj)
        {
            DependencyObject child = obj;
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(child);

                if (child is Window)
                    return child as Window;
                else
                    child = parent;
            }
        }
    }
}