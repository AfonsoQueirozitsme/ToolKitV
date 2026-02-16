using System;
using System.Windows;
using System.Windows.Controls;
using ToolKitV.Models;

namespace ToolKitV.Views
{
    public partial class LoginView : UserControl
    {
        public event EventHandler LoginSuccess;

        public LoginView()
        {
            InitializeComponent();
        }

        private async void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            string key = LicenseInput.Text;
            
            if (string.IsNullOrEmpty(key))
            {
                StatusLabel.Text = "Por favor, insira uma chave.";
                return;
            }

            StatusLabel.Foreground = System.Windows.Media.Brushes.Gray;
            StatusLabel.Text = "A validar...";
            CheckButton.IsButtonEnabledValue = false;

            var result = await AuthService.ValidateLicenseAsync(key);

            if (result.valid)
            {
                AuthService.SaveLicense(key);
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                StatusLabel.Foreground = System.Windows.Media.Brushes.IndianRed;
                StatusLabel.Text = result.message ?? "Licença Inválida.";
                CheckButton.IsButtonEnabledValue = true;
            }
        }
    }
}
