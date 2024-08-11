using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Relauncher.Core.Configs;
using Relauncher.Core.Relaunchs;
using Relauncher.Helper;
using Relauncher.Models;
using Relauncher.Relaunchs;
using Relauncher.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Vanara.PInvoke;
using Windows.System;
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

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public partial class GenshinSettingsViewModel : ObservableObject
{
    [RelayCommand]
    private async Task LaunchAsync()
    {
        GenshinConfigurations config = Configurations.Genshin.Get();
        GenshinLauncherOption option = new()
        {
            GamePath = null,
            WorkingDirectory = null,
            Account = null!, // TODO: Not supported
            Arguments = new GenshinArgumentsOption()
            {
                IsUseArguments = config.IsUseArguments,
                IsUseWindowModeExclusive = config.IsUseWindowModeExclusive,
                IsScreenFullscreen = config.IsScreenFullscreen,
                IsPopupwindow = config.IsPopupwindow,
                IsPlatformTypeCloudThirdPartyMobile = config.IsPlatformTypeCloudThirdPartyMobile,
                IsScreenWidth = config.IsScreenWidth,
                ScreenWidth = config.ScreenWidth,
                IsScreenHeight = config.IsScreenHeight,
                ScreenHeight = config.ScreenHeight,
                IsMonitor = config.IsMonitor,
                Monitor = config.Monitor,
            },
            Advance = new GenshinAdvanceOption()
            {
                IsDisnetLaunching = config.IsDisnetLaunching,
                IsUseHDR = config.IsUseHDR,
            },
            Linkage = new GenshinLinkageOption()
            {
                ReShadePath = config.ReShadePath,
                IsUseReShade = config.IsUseReShade,
                IsUseReShadeSilent = config.IsUseReShadeSilent,
                IsUseBetterGI = config.IsUseBetterGI,
            },
            Region = null,
            Unlocker = new GenshinUnlockerOption()
            {
                IsUnlockFps = config.IsUnlockFps,
                UnlockFps = config.UnlockFps,
                UnlockFpsMethod = config.UnlockFpsMethod,
            },
        };

        await GenshinLauncher.LaunchAsync(delayMs: 1000, relaunchMethod: GenshinRelaunchMethod.Kill, option: option);
    }

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
    [Description("[TEMP] Monitor ID - 1 = Monitor Index")]
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
    [Description("[TEMP] Monitor Index + 1 = Monitor ID")]
    private ObservableCollection<string> monitors = new(IdentifyMonitorWindow.GetAllMonitors().Select((m, i) => (i + 1).ToString()));

    [RelayCommand]
    private async Task IdentifyMonitorsAsync()
    {
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }

    [ObservableProperty]
    private bool isUnlockFps = Configurations.Genshin.Get().IsUnlockFps;

    partial void OnIsUnlockFpsChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUnlockFps = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private int unlockFps = Configurations.Genshin.Get().UnlockFps;

    partial void OnUnlockFpsChanged(int value)
    {
        var config = Configurations.Genshin.Get();
        config.UnlockFps = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private int unlockFpsMethod = Configurations.Genshin.Get().UnlockFpsMethod;

    partial void OnUnlockFpsMethodChanged(int value)
    {
        var config = Configurations.Genshin.Get();
        config.UnlockFpsMethod = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isShowFps = Configurations.Genshin.Get().IsShowFps;

    partial void OnIsShowFpsChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsShowFps = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isDisnetLaunching = Configurations.Genshin.Get().IsDisnetLaunching;

    partial void OnIsDisnetLaunchingChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsDisnetLaunching = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [RelayCommand]
    private async Task RestoreDisnetAsync()
    {
        await Task.Run(() =>
        {
            FluentProcess netsh2 = FluentProcess.Create()
                .FileName("netsh")
                .Arguments("advfirewall firewall delete rule name=\"DIS_GENSHIN_NETWORK\"")
                .CreateNoWindow()
                .UseShellExecute(false)
                .Verb("runas")
                .Start()
                .WaitForExit();
        });
    }

    [ObservableProperty]
    private bool isSquareCorner = Configurations.Genshin.Get().IsSquareCorner;

    partial void OnIsSquareCornerChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsSquareCorner = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isDarkMode = Configurations.Genshin.Get().IsDarkMode;

    partial void OnIsDarkModeChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsDarkMode = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isUseHDR = Configurations.Genshin.Get().IsUseHDR;

    partial void OnIsUseHDRChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseHDR = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private string reShadePath = Configurations.Genshin.Get().ReShadePath ?? string.Empty;

    partial void OnReShadePathChanged(string value)
    {
        var config = Configurations.Genshin.Get();
        config.ReShadePath = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [RelayCommand]
    private async Task SelectReShadePathAsync()
    {
        GenshinConfigurations config = Configurations.Genshin.Get();
        GenshinSettingsWindow? owner = null;

        if (System.Windows.Application.Current.Windows.OfType<GenshinSettingsWindow>() is { } wins)
        {
            owner = wins.FirstOrDefault();
        }

        if (Directory.Exists(config.ReShadePath))
        {
            var uiMessageBox = new Wpf.Ui.Controls.MessageBox()
            {
                Title = "提示",
                Content =
                    $"""
                    当前已正确设定 3DMigoto 目录“{config.ReShadePath}”。
                    您是否需要重新设定？
                    """,
                PrimaryButtonText = "是",
                CloseButtonText = "否",
                IsSecondaryButtonEnabled = false,
                PrimaryButtonAppearance = ControlAppearance.Info,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
            };

            var result = await uiMessageBox.ShowDialogAsync();

            if (result != Wpf.Ui.Controls.MessageBoxResult.Primary)
            {
                return;
            }
        }

        CommonOpenFileDialog dialog = new()
        {
            Title = "选择 3DMigoto 目录",
            IsFolderPicker = true,
            RestoreDirectory = true,
            InitialDirectory = config.ReShadePath,
            DefaultDirectory = config.ReShadePath,
        };

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            string selectedDirectory = dialog.FileName;

            if (!File.Exists(Path.Combine(selectedDirectory, "3DMigoto Loader.exe")))
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox()
                {
                    Title = "错误",
                    Content = "选择的目录下 3DMigoto Loader.exe 不存在",
                    CloseButtonText = "确定",
                    IsPrimaryButtonEnabled = false,
                    IsSecondaryButtonEnabled = false,
                    PrimaryButtonAppearance = ControlAppearance.Info,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = owner,
                };

                _ = await uiMessageBox.ShowDialogAsync();
                return;
            }

            ReShadePath = selectedDirectory;
        }
    }

    [RelayCommand]
    private async Task OpenLoaderFolderAsync()
    {
        if (Directory.Exists(ReShadePath))
        {
            _ = await Launcher.LaunchUriAsync(new Uri($"file://{ReShadePath}"));
        }
    }

    [ObservableProperty]
    private bool isUseReShade = Configurations.Genshin.Get().IsUseReShade;

    partial void OnIsUseReShadeChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseReShade = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isUseReShadeSilent = Configurations.Genshin.Get().IsUseReShadeSilent;

    partial void OnIsUseReShadeSilentChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseReShadeSilent = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [RelayCommand]
    private async Task OpenFischlessWebsiteAsync()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("https://github.com/GenshinMatrix/Fischless"));
    }

    [RelayCommand]
    private async Task LaunchFischlessAsync()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("fischless://"));
    }

    [ObservableProperty]
    private bool isUseBetterGI = Configurations.Genshin.Get().IsUseBetterGI;

    partial void OnIsUseBetterGIChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseBetterGI = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [RelayCommand]
    private async Task OpenBetterGIWebsiteAsync()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("https://github.com/babalae/better-genshin-impact"));
    }

    [RelayCommand]
    private async Task LaunchBetterGIAsync()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("bettergi://"));
    }

    [ObservableProperty]
    private bool isUseBorderless = Configurations.Genshin.Get().IsUseBorderless;

    partial void OnIsUseBorderlessChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseBorderless = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [RelayCommand]
    public async Task EnableWindowBorderlessAsync()
    {
        // TODO
        await Task.CompletedTask;
        //if (!await GILauncher.TryGetProcessAsync(async t =>
        //{
        //    if (!RuntimeHelper.IsElevated)
        //    {
        //        if (MessageBoxX.Question(MuiLanguage.Mui("UACRequestRestartHint")) == MessageBoxResult.Yes)
        //        {
        //            RuntimeHelper.RestartAsElevated();
        //        }
        //        return;
        //    }

        //    await Task.CompletedTask;

        //    nint hWnd = t.MainWindowHandle;
        //    hWnd.EnableWindowBorderless();
        //}))
        //{
        //    // NO GAME PLAYING
        //}
    }

    [RelayCommand]
    public async Task DisableWindowBorderlessAsync()
    {
        // TODO
        await Task.CompletedTask;
        //if (!await GILauncher.TryGetProcessAsync(async t =>
        //{
        //    if (!RuntimeHelper.IsElevated)
        //    {
        //        if (MessageBoxX.Question(MuiLanguage.Mui("UACRequestRestartHint")) == MessageBoxResult.Yes)
        //        {
        //            RuntimeHelper.RestartAsElevated();
        //        }
        //        return;
        //    }

        //    await Task.CompletedTask;

        //    nint hWnd = t.MainWindowHandle;
        //    hWnd.DisableWindowBorderless();
        //}))
        //{
        //    // NO GAME PLAYING
        //}
    }

    [RelayCommand]
    public async Task EnableWindowTopmostAsync()
    {
        // TODO
        await Task.CompletedTask;
        //if (!await GILauncher.TryGetProcessAsync(async t =>
        //{
        //    if (!RuntimeHelper.IsElevated)
        //    {
        //        if (MessageBoxX.Question(MuiLanguage.Mui("UACRequestRestartHint")) == MessageBoxResult.Yes)
        //        {
        //            RuntimeHelper.RestartAsElevated();
        //        }
        //        return;
        //    }

        //    await Task.CompletedTask;

        //    nint hWnd = t.MainWindowHandle;
        //    hWnd.EnableWindowTopmost();
        //}))
        //{
        //    // NO GAME PLAYING
        //}
    }

    [RelayCommand]
    public async Task DisableWindowTopmostAsync()
    {
        // TODO
        await Task.CompletedTask;
        //if (!await GILauncher.TryGetProcessAsync(async t =>
        //{
        //    if (!RuntimeHelper.IsElevated)
        //    {
        //        if (MessageBoxX.Question(MuiLanguage.Mui("UACRequestRestartHint")) == MessageBoxResult.Yes)
        //        {
        //            RuntimeHelper.RestartAsElevated();
        //        }
        //        return;
        //    }

        //    await Task.CompletedTask;

        //    nint hWnd = t.MainWindowHandle;
        //    hWnd.DisableWindowTopmost();
        //}))
        //{
        //    // NO GAME PLAYING
        //}
    }

    [RelayCommand]
    public void EnableWindowDragMove()
    {
        // TODO
        //DragMoveProvider.IsEnabled = true;
    }

    [RelayCommand]
    public void DisableWindowDragMove()
    {
        // TODO
        //DragMoveProvider.IsEnabled = false;
    }

    [RelayCommand]
    public async Task EnableWindowMaximizeBoxAsync()
    {
        // TODO
        await Task.CompletedTask;
        //if (!await GILauncher.TryGetProcessAsync(async t =>
        //{
        //    if (!RuntimeHelper.IsElevated)
        //    {
        //        if (MessageBoxX.Question(MuiLanguage.Mui("UACRequestRestartHint")) == MessageBoxResult.Yes)
        //        {
        //            RuntimeHelper.RestartAsElevated();
        //        }
        //        return;
        //    }

        //    await Task.CompletedTask;

        //    nint hWnd = t.MainWindowHandle;
        //    hWnd.EnableWindowMaximizeBox();
        //}))
        //{
        //    // NO GAME PLAYING
        //}
    }

    [RelayCommand]
    public async Task RestoreWindowPositonAsync()
    {
        // TODO
        await Task.CompletedTask;
        //if (!await GILauncher.TryGetProcessAsync(async t =>
        //{
        //    if (!RuntimeHelper.IsElevated)
        //    {
        //        if (MessageBoxX.Question(MuiLanguage.Mui("UACRequestRestartHint")) == MessageBoxResult.Yes)
        //        {
        //            RuntimeHelper.RestartAsElevated();
        //        }
        //        return;
        //    }

        //    await Task.CompletedTask;

        //    nint hWnd = t.MainWindowHandle;
        //    hWnd.RestoreWindowPositon();
        //}))
        //{
        //    // NO GAME PLAYING
        //}
    }

    [ObservableProperty]
    private bool isUseQuickScreenshot = Configurations.Genshin.Get().IsUseQuickScreenshot;

    partial void OnIsUseQuickScreenshotChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseQuickScreenshot = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isUseQuickScreenshotHideUID = Configurations.Genshin.Get().IsUseQuickScreenshotHideUID;

    partial void OnIsUseQuickScreenshotHideUIDChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseQuickScreenshotHideUID = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [ObservableProperty]
    private bool isUseAutoMute = Configurations.Genshin.Get().IsUseAutoMute;

    partial void OnIsUseAutoMuteChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseAutoMute = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }

    [RelayCommand]
    public async Task LaunchWindowsSettingsAppsVolumeAsync()
    {
        _ = await Launcher.LaunchUriAsync(new Uri("ms-settings:apps-volume"));
    }

    [ObservableProperty]
    private bool isUseAutoClick = Configurations.Genshin.Get().IsUseAutoClick;

    partial void OnIsUseAutoClickChanged(bool value)
    {
        var config = Configurations.Genshin.Get();
        config.IsUseAutoClick = value;
        Configurations.Genshin.Set(config);
        ConfigurationManager.Save();
    }
}
