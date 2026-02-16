using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace ToolKitV
{
    public partial class App : Application
    {
        protected Mutex Mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            Mutex = new Mutex(true, ResourceAssembly.GetName().Name);

            if (!Mutex.WaitOne(0, false))
            {
                Current.Shutdown();
                return;
            }
            else
            {
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Ocorreu um erro inesperado:\n" + e.Exception.Message + "\n\nSe precisares de ajuda, entra no nosso Discord.\nFTW Studios Services", "Kit de Ferramentas Crash", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
