using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Relauncher.Core.Configs;
using Relauncher.Helper;
using Relauncher.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Vanara.PInvoke;
using Wpf.Ui.Controls;
using MouseButtonState = System.Windows.Input.MouseButtonState;

namespace Relauncher.Views;

[ObservableObject]
public partial class GenshinSettingsWindow : Window
{
    public nint Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);
    public nint TargetHWnd { get; set; }
    public GenshinSettingsViewModel ViewModel { get; } = new();

    public GenshinSettingsWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (TargetHWnd != IntPtr.Zero)
        {
            _ = User32.EnableWindow(TargetHWnd, true);
            _ = User32.SetActiveWindow(TargetHWnd);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (WindowBackdrop.IsSupported(WindowBackdropType.Mica))
        {
            WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        }

        if (TargetHWnd != IntPtr.Zero)
        {
            _ = User32.EnableWindow(TargetHWnd, false);

            _ = User32.GetWindowRect(TargetHWnd, out RECT pos);
            _ = User32.GetClientRect(TargetHWnd, out RECT rect);

            int width = (int)DpiHelper.CalcDPI(Width);
            int height = (int)DpiHelper.CalcDPI(Height);

            _ = User32.SetWindowPos(Handle, IntPtr.Zero, (int)(pos.Left + ((rect.Width - width) / 2d)), (int)(pos.Top + ((rect.Height - height) / 2d)), width, height, User32.SetWindowPosFlags.SWP_SHOWWINDOW);
        }
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    public void ShowDialog(HWND targetHWnd)
    {
        TargetHWnd = targetHWnd.DangerousGetHandle();
        Show();
    }

    [RelayCommand]
    private void CloseWindow()
    {
        Close();
    }
}

public partial class GenshinSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isUseArguments = Configurations.Genshin.Get().IsUseArguments;

    partial void OnIsUseArgumentsChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseArguments = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-window-mode exclusive")]
    private bool isUseWindowModeExclusive = Configurations.Genshin.Get().IsUseWindowModeExclusive;

    partial void OnIsUseWindowModeExclusiveChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseWindowModeExclusive = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-screen-fullscreen")]
    private bool isScreenFullscreen = Configurations.Genshin.Get().IsScreenFullscreen;

    partial void OnIsScreenFullscreenChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsScreenFullscreen = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-popupwindow")]
    private bool isPopupwindow = Configurations.Genshin.Get().IsPopupwindow;

    partial void OnIsPopupwindowChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsPopupwindow = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-platform_type CLOUD_THIRD_PARTY_MOBILE")]
    private bool isPlatformTypeCloudThirdPartyMobile = Configurations.Genshin.Get().IsPlatformTypeCloudThirdPartyMobile;

    partial void OnIsPlatformTypeCloudThirdPartyMobileChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsPlatformTypeCloudThirdPartyMobile = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private ObservableCollection<string> screenPreset =
    [
    ];

    [ObservableProperty]
    private object screenPresetValue = "";

    partial void OnScreenPresetValueChanged(object value)
    {
        string screenPreset = ((dynamic)value).Content;
        string[] screenPresets = screenPreset.Split('x', ':');

        if (screenPresets.Length == 2)
        {
            ScreenWidth = int.Parse(screenPresets[0]);
            ScreenHeight = int.Parse(screenPresets[1]);
        }
    }

    [ObservableProperty]
    [property: Description("-screen-width")]
    private bool isScreenWidth = Configurations.Genshin.Get().IsScreenWidth;

    partial void OnIsScreenWidthChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsScreenWidth = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-screen-width")]
    private int screenWidth = Configurations.Genshin.Get().ScreenWidth;

    partial void OnScreenWidthChanged(int value)
    {
        var config = Configurations.Genshin.Get();
        config.ScreenWidth = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-screen-width")]
    private bool isScreenHeight = Configurations.Genshin.Get().IsScreenHeight;

    partial void OnIsScreenHeightChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsScreenHeight = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-screen-height")]
    private int screenHeight = Configurations.Genshin.Get().ScreenHeight;

    partial void OnScreenHeightChanged(int value)
    {
        var config = Configurations.Genshin.Get();
        config.ScreenHeight = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [property: Description("-monitor")]
    private bool isMonitor = Configurations.Genshin.Get().IsMonitor;

    partial void OnIsMonitorChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsMonitor = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    [Description("Monitor ID - 1 = Monitor Index")]
    [property: Description("-monitor")]
    private int monitor = Configurations.Genshin.Get().Monitor - 1;

    partial void OnMonitorChanged(int value)
    {
        var config = Configurations.Genshin.Get();
        config.Monitor = value + 1;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private ObservableCollection<string> monitors = new(IdentifyMonitorWindow.GetAllMonitors().Select((m, i) => (i + 1).ToString()));

    [RelayCommand]
    private async Task IdentifyMonitorsAsync()
    {
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }
}
