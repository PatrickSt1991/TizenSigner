using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace TizenSigner
{
    public class TizenPusher : INotifyPropertyChanged
    {
        public ICommand PushToDevice { get; }
        private readonly string _tizenCliPath;
        private readonly Func<string> _getDeviceAddress;

        public TizenPusher(string tizenCliPath, Func<string> getDeviceAddress)
        {
            _tizenCliPath = tizenCliPath;
            _getDeviceAddress = getDeviceAddress;
            PushToDevice = new RelayCommand(ExecutePushToDevice, CanPushToDevice);
        }

        private async void ExecutePushToDevice(object parameter)
        {
            var address = _getDeviceAddress();
            var studioRoot = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(_tizenCliPath)).FullName).FullName;
            var sdbPath = Path.Combine(studioRoot, "sdb.exe");
            var tizenPath = _tizenCliPath;

            RunCommand(sdbPath, $"connect {address}");

            string tvName = GetTvName(sdbPath);

            if (string.IsNullOrEmpty(tvName))
            {
                MessageBox.Show("TV Naam kon niet worden gevonden...");
                return;
            }

            string wgtFile = "ClubInfoBoard.wgt";
            using var client = new HttpClient();
            var response = await client.GetAsync("https://github.com/PatrickSt1991/Sportlink.Club.Info.Viewer/releases/latest/download/ClubInfoBoard.wgt");
            response.EnsureSuccessStatusCode();
            await using var fs = new FileStream(wgtFile, FileMode.Create);
            await response.Content.CopyToAsync(fs);

            string fullWgtPath = Path.GetFullPath(wgtFile);

            UpdateProfileCertificatePaths();


            RunCommand(tizenPath, $"package -t wgt -s custom -- {fullWgtPath}");

            //RunCommand(tizenPath, $"install -n {fullWgtPath} -t {tvName}");
        }

        void UpdateProfileCertificatePaths()
        {
            string profilePath = Path.GetFullPath("TizenTools/profile/profiles.xml");
            var xml = XDocument.Load(profilePath);
            var profileItems = xml.Descendants("profileitem").ToList();

            foreach (var item in profileItems)
            {
                string distributor = item.Attribute("distributor")?.Value;

                if (distributor == "0")
                    item.SetAttributeValue("key", Path.GetFullPath("TizenTools/profile/author.p12"));

                if (distributor == "1")
                    item.SetAttributeValue("key", Path.GetFullPath("TizenTools/profile/distributor.p12"));
            }

            xml.Save(profilePath);
        }

        private bool CanPushToDevice(object parameter)
        {
            return !string.IsNullOrEmpty(_tizenCliPath) && 
                !string.IsNullOrEmpty(_getDeviceAddress());
        }
        private string GetTvName(string sdbPath)
        {
            var output = RunCommand(sdbPath, "devices");
            var match = Regex.Match(output, @"(?<=\n)(?<device>[^\s]+)\s+device");

            return match.Success ? match.Groups["device"].Value.Trim() : "";
        }
        private string RunCommand(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using var proc = Process.Start(psi)!;
            string output = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            if (proc.ExitCode != 0)
                throw new Exception($"Command failed: {fileName} {arguments}\nOutput: {output}\nError: {error}");

            return output;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
