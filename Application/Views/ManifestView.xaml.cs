using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ToolKitV.Views
{
    public partial class ManifestView : UserControl
    {
        public ManifestView()
        {
            InitializeComponent();
        }

        private void GenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = ResourceFolder.Path;
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                MessageBox.Show("Por favor, seleciona uma pasta válida!", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string manifestContent = GenerateManifestContent(path);
                File.WriteAllText(Path.Combine(path, "fxmanifest.lua"), manifestContent);
                ResultPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar manifesto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateManifestContent(string path)
        {
            string desc = ResourceDescription.Text;
            bool hasStream = Directory.Exists(Path.Combine(path, "stream")) || 
                             Directory.GetFiles(path, "*.ytd", SearchOption.AllDirectories).Any() ||
                             Directory.GetFiles(path, "*.yft", SearchOption.AllDirectories).Any();

            string manifest = $@"fx_version 'cerulean'
game 'gta5'

author 'FTW Studios'
description '{desc}'
version '1.0.0'

";
            if (hasStream)
            {
                manifest += "data_file 'HANDLING_FILE' 'data/**/handling.meta'\n";
                manifest += "data_file 'VEHICLE_METADATA_FILE' 'data/**/vehicles.meta'\n";
                manifest += "data_file 'CARCOLS_FILE' 'data/**/carcols.meta'\n";
                manifest += "data_file 'VEHICLE_VARIATION_FILE' 'data/**/carvariations.meta'\n";
            }

            manifest += "\n-- Gerado automaticamente pelo Kit de Ferramentas FTW Studios\n";
            return manifest;
        }
    }
}
