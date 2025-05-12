using System.Windows;

namespace TizenSigner;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var installer = new TizenInstaller();

            if (!await installer.EnsureTizenCliAvailable())
            {
                MessageBox.Show("Tizen tools zijn benodigd voor deze applicatie.",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            var mainWindow = new MainWindow(installer.TizenCliPath);
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Application failed to start: {ex.Message}", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }
}

