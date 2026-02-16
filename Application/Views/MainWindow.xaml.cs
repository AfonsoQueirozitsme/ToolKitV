using System;
using System.Windows;
using System.Windows.Input;
using ToolKitV.Models;

namespace ToolKitV
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckLicenseOnStartup();
        }

        private async void CheckLicenseOnStartup()
        {
            LoadingControl.Visibility = Visibility.Visible;
            LoginControl.Visibility = Visibility.Collapsed;
            LoadingControl.SetStatus("Verificando Licença...");

            string storedKey = AuthService.GetStoredLicense();
            
            if (!string.IsNullOrEmpty(storedKey))
            {
                var result = await AuthService.ValidateLicenseAsync(storedKey);
                if (result.valid)
                {
                    LoadingControl.SetStatus("Bem-vindo, " + (result.owner?.username ?? "Utilizador"));
                    await System.Threading.Tasks.Task.Delay(1000); // Small delay to show success
                    OnAuthSuccess();
                    return;
                }
            }
            
            // If no license or invalid, show LoginView
            await System.Threading.Tasks.Task.Delay(500); // Smooth transition
            LoadingControl.Visibility = Visibility.Collapsed;
            LoginControl.Visibility = Visibility.Visible;
        }

        private void OnAuthSuccess()
        {
            AuthOverlay.Visibility = Visibility.Collapsed;
            MainContentGrid.Visibility = Visibility.Visible;
        }

        private void LoginControl_LoginSuccess(object sender, EventArgs e)
        {
            OnAuthSuccess();
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void NavigateTo(string tag)
        {
            if (string.IsNullOrEmpty(tag) || OptimizeView == null) return;

            // Reset visibility
            OptimizeView.Visibility = Visibility.Collapsed;
            DiagnosticView.Visibility = Visibility.Collapsed;
            ManifestView.Visibility = Visibility.Collapsed;
            RenameView.Visibility = Visibility.Collapsed;

            switch (tag)
            {
                case "OPTIMIZE":
                    PageTitle.Text = "OTIMIZADOR DE TEXTURAS";
                    OptimizeView.Visibility = Visibility.Visible;
                    break;
                case "DIAGNOSTIC":
                    PageTitle.Text = "DIAGNÓSTICO DE PERFORMANCE";
                    DiagnosticView.Visibility = Visibility.Visible;
                    break;
                case "MANIFEST":
                    PageTitle.Text = "GERADOR DE MANIFESTO";
                    ManifestView.Visibility = Visibility.Visible;
                    break;
                case "RENAME":
                    PageTitle.Text = "RENOMEAR ASSETS (BULK)";
                    RenameView.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
