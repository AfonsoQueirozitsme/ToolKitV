using System.Windows;
using System.Windows.Controls;
using ToolKitV;

namespace ToolKitV.Views
{
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView lv && lv.SelectedItem is ListViewItem item)
            {
                var tag = item.Tag?.ToString();
                (Application.Current.MainWindow as MainWindow)?.NavigateTo(tag);
            }
        }
    }
}
