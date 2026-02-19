using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ToolKitV.Views
{
    public partial class MergerView : UserControl
    {
        private ObservableCollection<string> logs = new ObservableCollection<string>();

        public MergerView()
        {
            InitializeComponent();
            LogList.ItemsSource = logs;
        }

        private async void MergeBtn_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = SourceRootFolder.Path;
            string exportRoot = ExportFolder.Path;
            string newResourceName = MergedResourceName.Text;

            if (string.IsNullOrEmpty(sourcePath) || !Directory.Exists(sourcePath))
            {
                MessageBox.Show("Seleciona a pasta de origem!");
                return;
            }

            if (string.IsNullOrEmpty(exportRoot) || !Directory.Exists(exportRoot))
            {
                MessageBox.Show("Seleciona a pasta de destino!");
                return;
            }

            string finalPath = Path.Combine(exportRoot, newResourceName);
            string streamPath = Path.Combine(finalPath, "stream");
            string dataPath = Path.Combine(finalPath, "data");

            MergeProgressPanel.Visibility = Visibility.Visible;
            LogList.Visibility = Visibility.Visible;
            logs.Clear();
            MergeProgressBar.Width = 0;
            MergeBtn.IsEnabled = false;

            try
            {
                if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
                if (!Directory.Exists(streamPath)) Directory.CreateDirectory(streamPath);
                if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);

                await Task.Run(() => PerformMerge(sourcePath, streamPath, dataPath));

                MergeStatusTitle.Text = "MERGE CONCLUÍDO!";
                MergeStatusDesc.Text = $"Os ficheiros foram unidos em {newResourceName}. Lembra-te de verificar o manifesto.";
                MergeProgressBar.Width = 300; 
                GenerateMergedManifest(finalPath, newResourceName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro durante o merge: " + ex.Message);
            }
            finally
            {
                MergeBtn.IsEnabled = true;
            }
        }

        private void PerformMerge(string source, string targetStream, string targetData)
        {
            var allFiles = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            int processedCount = 0;

            foreach (var file in allFiles)
            {
                string ext = Path.GetExtension(file).ToLower();
                string fileName = Path.GetFileName(file);
                
                if (fileName.Contains("fxmanifest") || fileName.Contains("__resource")) continue;

                try
                {
                    // Determinar o nome do sub-recurso (carro/mapa/etc)
                    string relativePath = Path.GetRelativePath(source, file);
                    string[] parts = relativePath.Split(Path.DirectorySeparatorChar);
                    string subResourceName = parts.Length > 1 ? parts[0] : "root_assets";

                    // Aplicar [] ao nome da pasta se ainda não tiver
                    if (!subResourceName.StartsWith("[") || !subResourceName.EndsWith("]"))
                    {
                        subResourceName = $"[{subResourceName}]";
                    }

                    // Stream Files
                    if (new[] { ".ytd", ".yft", ".ydp", ".ybn", ".ydr", ".ypt", ".ymap", ".ytyp" }.Contains(ext))
                    {
                        string subStreamPath = Path.Combine(targetStream, subResourceName);
                        if (!Directory.Exists(subStreamPath)) Directory.CreateDirectory(subStreamPath);
                        
                        string dest = Path.Combine(subStreamPath, fileName);
                        File.Copy(file, dest, true);
                        AddLog($"[STREAM] {subResourceName} -> {fileName}");
                    }
                    // Data Files
                    else if (new[] { ".meta", ".xml", ".dat", ".lua" }.Contains(ext))
                    {
                        string subDataPath = Path.Combine(targetData, subResourceName);
                        if (!Directory.Exists(subDataPath)) Directory.CreateDirectory(subDataPath);
                        
                        string dest = Path.Combine(subDataPath, fileName);
                        File.Copy(file, dest, true);
                        AddLog($"[DATA] {subResourceName} -> {fileName}");
                    }
                }
                catch (Exception ex)
                {
                    AddLog($"ERRO em {fileName}: {ex.Message}");
                }

                processedCount++;
                double progress = (double)processedCount / allFiles.Length;
                Application.Current.Dispatcher.Invoke(() => {
                    MergeProgressBar.Width = progress * 300;
                });
            }
        }

        private void GenerateMergedManifest(string path, string name)
        {
            string manifest = $@"fx_version 'cerulean'
game 'gta5'

author 'FTW Studios (Merged)'
description 'Pack unido e organizado com Coleções []'
version '1.0.0'

-- DATA FILES (Organizados por Sub-Pasta [])
data_file 'HANDLING_FILE' 'data/**/*.meta'
data_file 'VEHICLE_METADATA_FILE' 'data/**/*.meta'
data_file 'CARCOLS_FILE' 'data/**/*.meta'
data_file 'VEHICLE_VARIATION_FILE' 'data/**/*.meta'
data_file 'VEHICLE_LAYOUTS_FILE' 'data/**/*.meta'

client_script 'data/**/*.lua'
";
            File.WriteAllText(Path.Combine(path, "fxmanifest.lua"), manifest);
        }

        private void AddLog(string msg)
        {
            Application.Current.Dispatcher.Invoke(() => {
                logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {msg}");
                if (logs.Count > 50) logs.RemoveAt(50);
            });
        }
    }
}
