using System.Windows.Controls;

namespace ToolKitV.Views
{
    public partial class LoadingView : UserControl
    {
        public LoadingView()
        {
            InitializeComponent();
        }

        public void SetStatus(string text)
        {
            StatusText.Text = text.ToUpper();
        }
    }
}
