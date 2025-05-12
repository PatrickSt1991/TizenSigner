using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace TizenSigner;

public partial class MainWindow : Window
{
    private readonly string _tizenCliPath;
    private string _deviceAddress;

    public string DeviceAddress
    {
        get => _deviceAddress;
        set
        {
            if (_deviceAddress != value)
            {
                _deviceAddress = value;
                OnPropertyChanged(nameof(DeviceAddress));
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
    public TizenPusher DevicePusher { get; }
    public ICommand PushToDevice => DevicePusher.PushToDevice;
    public MainWindow(string tizenCliPath)
    {
        InitializeComponent();
        DevicePusher = new TizenPusher(tizenCliPath, () => this.DeviceAddress);
        this.DataContext = this;
        _tizenCliPath = tizenCliPath;

        if (!File.Exists(_tizenCliPath))
        {
            MessageBox.Show("Tizen CLI niet gevonden op standaard locaties...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}