namespace Flow.Launcher.Plugin.IPDetails.Settings;

public partial class SettingsControl
{
    private readonly Settings _settings;

    public string ApiKey
    {
        get => _settings.ApiKey;
        set => _settings.ApiKey = value;
    }

    public SettingsControl(Settings settings)
    {
        _settings = settings;
        
        DataContext = this;

        InitializeComponent();
    }
}