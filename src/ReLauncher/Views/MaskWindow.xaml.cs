using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Relauncher.Extensions;
using Relauncher.Threading;
using System.Windows;
using System.Windows.Interop;
using Vanara.PInvoke;
using Application = System.Windows.Application;

namespace Relauncher.Views;

[ObservableObject]
public partial class MaskWindow : Window
{
    public nint Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

    public MaskWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        this.SetChildWindow();
        this.HideFromAltTab();
    }

    [RelayCommand]
    private void OpenSettings()
    {
        var targetHWnd = User32.GetParent(Handle);
        Application.Current.Windows.OfType<GenshinSettingsWindow>().ToList().ForEach(x => x.Close());
        new GenshinSettingsWindow().ShowDialog(targetHWnd);
    }

    public static MaskWindow CreateWindow()
    {
        return UIDispatcherHelper.Invoke(() => new MaskWindow() { Visibility = Visibility.Visible });
    }
}
