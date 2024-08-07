using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Relauncher.Helper;
using System.ComponentModel;
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

    [RelayCommand]
    private async Task IdentifyMonitorsAsync()
    {
        await IdentifyMonitorWindow.IdentifyAllMonitorsAsync(3).ConfigureAwait(false);
    }
}
