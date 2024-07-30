using System.Windows;
using System.Windows.Controls;

namespace DiscordJob
{
    public class EmptyItemTemplateSelector: DataTemplateSelector
    {
        public required DataTemplate EmptyTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            // 如果ItemsSource为空或者Count为0，则使用EmptyTemplate
            var listBox = ItemsControl.ItemsControlFromItemContainer(container);
            return listBox.Items.Count == 0 ? EmptyTemplate : null;
        }
    }
}
