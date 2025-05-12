using System.Diagnostics;
using System.IO;
using System.Windows;

namespace TizenSigner
{
    public class TizenInstaller
    {
        private const string InstallerFileName = "web-cli_Tizen_Studio_5.5_windows-64.exe";
        private readonly string _appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private string? _tizenCliPath;

        public string TizenCliPath => _tizenCliPath ??= FindTizenCliPath();

        private static readonly string[] PossibleTizenPaths =
        [
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Tizen Studio"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Tizen Studio"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TizenStudio"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "TizenStudio"),
            "C:\\tizen-studio",
            "C:\\TizenStudioCli",
            Environment.GetEnvironmentVariable("TIZEN_STUDIO_HOME") ?? string.Empty
        ];

        public async Task<bool> EnsureTizenCliAvailable()
        {
            if (File.Exists(TizenCliPath))
                return true;

            return await InstallMinimalCli();
        }

        private static string? FindTizenCliPath()
        {
            foreach (var basePath in PossibleTizenPaths)
            {
                if (string.IsNullOrEmpty(basePath)) continue;

                var possiblePath = Path.Combine(basePath, "tools", "ide", "bin", "tizen.bat");

                if (File.Exists(possiblePath))
                    return possiblePath;
            }
            return null;
        }

        private async Task<bool> InstallMinimalCli()
        {
            string installerPath = Path.Combine(_appDirectory, "TizenTools", InstallerFileName);

            if (!File.Exists(installerPath))
            {
                MessageBox.Show("Tizen CLI installer not found. Please reinstall the application.",
                              "Missing Components",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                return false;
            }

            var result = MessageBox.Show(
                "This application requires Tizen CLI tools (~300MB). Install to default location now?",
                "Installation Required",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return false;
            }

            var progressDialog = new InstallationProgressWindow();
            progressDialog.Show();

            try
            {
                bool success = await RunSilentInstall(installerPath);

                progressDialog.Close();

                if (!success)
                {
                    MessageBox.Show("Installation failed. Please try again or install Tizen Studio manually.",
                                  "Installation Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                    return false;
                }

                _tizenCliPath = FindTizenCliPath();
                return _tizenCliPath != null;
            }
            catch (Exception ex)
            {
                progressDialog.Close();
                MessageBox.Show($"Installation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static async Task<bool> RunSilentInstall(string installerPath)
        {
            try
            {

                string installPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),"TizenStudio");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = installerPath,
                        Arguments = $"/S --accept-license \"{installPath}\"",
                        Verb = "runas",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();
                return process.ExitCode == 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Installation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}