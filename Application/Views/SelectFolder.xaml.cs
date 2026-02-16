using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace ToolKitV.Views
{
    public partial class SelectFolder : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; } = "";

        public string PathValue { get; set; } = "";
        public string Path
        {
            get => PathValue;
            set
            {
                if (value != PathValue)
                {
                    PathValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SelectFolder()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WinForms.FolderBrowserDialog FBD = new WinForms.FolderBrowserDialog();
            if (FBD.ShowDialog() == WinForms.DialogResult.OK)
            {
                Path = FBD.SelectedPath;
            }
        }

        private void Border_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                InputBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("PrimaryAccentBrush");
                InputBorder.Background = (System.Windows.Media.Brush)FindResource("GlassHoverBrush");
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            InputBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("LineBrush");
            InputBorder.Background = (System.Windows.Media.Brush)FindResource("SidebarBgBrush");
            e.Handled = true;
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            InputBorder.BorderBrush = (System.Windows.Media.Brush)FindResource("LineBrush");
            InputBorder.Background = (System.Windows.Media.Brush)FindResource("SidebarBgBrush");

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string droppedPath = files[0];
                    if (Directory.Exists(droppedPath))
                    {
                        Path = droppedPath;
                    }
                    else if (File.Exists(droppedPath))
                    {
                        Path = System.IO.Path.GetDirectoryName(droppedPath);
                    }
                }
            }
            e.Handled = true;
        }
    }
}
