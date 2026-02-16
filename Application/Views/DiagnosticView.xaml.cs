using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ToolKitV.Views
{
    public partial class DiagnosticView : UserControl
    {
        public class DiagnosticIssue
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public DiagnosticView()
        {
            InitializeComponent();
        }

        private void ScanBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = ScanFolder.Path;
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

            var issues = RunDiagnostic(path);
            DisplayResults(issues);
        }

        private List<DiagnosticIssue> RunDiagnostic(string path)
        {
            var issues = new List<DiagnosticIssue>();
            var ytdFiles = Directory.GetFiles(path, "*.ytd", SearchOption.AllDirectories);

            foreach (var ytd in ytdFiles)
            {
                var info = new FileInfo(ytd);
                if (info.Length > 16 * 1024 * 1024) // 16MB
                {
                    issues.Add(new DiagnosticIssue 
                    { 
                        Title = $"DICIONÁRIO PESADO: {info.Name}", 
                        Description = $"Este ficheiro tem {info.Length / 1024 / 1024}MB. Ficheiros acima de 16MB causam instabilidade no streaming do FiveM." 
                    });
                }
            }

            var ydrs = Directory.GetFiles(path, "*.ydr", SearchOption.AllDirectories);
            if (ydrs.Length > 0 && !Directory.GetFiles(path, "*_lod*", SearchOption.AllDirectories).Any())
            {
                issues.Add(new DiagnosticIssue 
                { 
                    Title = "FALTA DE LODS DETETADA", 
                    Description = "Modelos 3D sem LODS (Level of Detail) causam queda excessiva de FPS quando o objeto é visto de longe." 
                });
            }

            return issues;
        }

        private void DisplayResults(List<DiagnosticIssue> issues)
        {
            DiagnosticResult.Visibility = Visibility.Visible;
            IssuesList.Visibility = issues.Any() ? Visibility.Visible : Visibility.Collapsed;
            IssuesList.ItemsSource = issues;

            if (issues.Count == 0)
            {
                ScoreLabel.Text = "A+";
                ScoreLabel.Foreground = Brushes.LightGreen;
                DiagnosisTitle.Text = "OTIMIZAÇÃO PERFEITA";
                DiagnosisDesc.Text = "Não foram encontrados problemas de carregamento ou performance nesta pasta.";
            }
            else if (issues.Count < 3)
            {
                ScoreLabel.Text = "B";
                ScoreLabel.Foreground = Brushes.Yellow;
                DiagnosisTitle.Text = "NECESSITA ATENÇÃO SUAVE";
                DiagnosisDesc.Text = "Existem alguns ficheiros que podem ser otimizados para melhorar a estabilidade.";
            }
            else
            {
                ScoreLabel.Text = "D";
                ScoreLabel.Foreground = Brushes.OrangeRed;
                DiagnosisTitle.Text = "RISCO DE CRASH/LAG";
                DiagnosisDesc.Text = "Foram encontrados múltiplos problemas críticos que vão afetar a performance do servidor.";
            }
        }
    }
}
