using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ToolKitV.Views
{
    public partial class RenameView : UserControl
    {
        public RenameView()
        {
            InitializeComponent();
        }

        private void RenameBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = TargetFolder.Path;
            string prefix = PrefixText.Text;

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Seleciona uma pasta válida!", "Erro");
                return;
            }

            try
            {
                int count = 0;
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    if (!fileName.StartsWith(prefix))
                    {
                        string directory = Path.GetDirectoryName(file);
                        string newPath = Path.Combine(directory, prefix + fileName);
                        File.Move(file, newPath);
                        count++;
                    }
                }

                RenameResultText.Text = $"{count} ficheiros renomeados com o prefixo '{prefix}'.";
                RenameResult.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao renomear: {ex.Message}");
            }
        }
    }
}
