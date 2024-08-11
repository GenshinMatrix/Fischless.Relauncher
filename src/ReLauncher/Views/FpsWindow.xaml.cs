using CommunityToolkit.Mvvm.ComponentModel;
using PresentMonFps;
using Relauncher.Extensions;
using Relauncher.Threading;
using System.Windows;
using System.Windows.Interop;

namespace Relauncher.Views;

[ObservableObject]
public partial class FpsWindow : Window
{
    [ObservableProperty]
    private string fps = 0.ToString();

    [ObservableProperty]
    private uint pid = 0;

    partial void OnPidChanged(uint value)
    {
        if (value != 0)
        {
            _ = Task.Run(async () =>
            {
                await FpsInspector.StartForeverAsync(new FpsRequest(Pid), (result) =>
                {
                    Fps = $"{result.Fps:0}";

                    if (Pid == 0)
                    {
                        result.IsCanceled = true;
                    }
                });
            });
        }
    }

    public nint Handle => Dispatcher.Invoke(() => new WindowInteropHelper(this).Handle);

    public FpsWindow()
    {
        DataContext = this;
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        this.SetLayeredWindow();
        this.SetChildWindow();
        this.HideFromAltTab();
    }

    public static FpsWindow CreateWindow(uint pid = 0)
    {
        return UIDispatcherHelper.Invoke(() => new FpsWindow() { Pid = pid });
    }
}
