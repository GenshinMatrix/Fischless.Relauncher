using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Vanara.PInvoke;

namespace Fischless.Relauncher.Views;

public partial class AutoClickStatusWindow : Window
{
    private const int DisplayDurationMilliseconds = 2000;
    private const int AnimationDurationMilliseconds = 180;

    private static AutoClickStatusWindow? _instance;
    private CancellationTokenSource? _hideCancellation;

    private AutoClickStatusWindow()
    {
        InitializeComponent();
    }

    public static void ShowStatus(bool isEnabled)
    {
        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            _instance ??= new AutoClickStatusWindow();
            _instance.ShowStatusCore(isEnabled);
        });
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        nint hWnd = new WindowInteropHelper(this).Handle;
        int style = User32.GetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE);
        style |= (int)(User32.WindowStylesEx.WS_EX_NOACTIVATE
            | User32.WindowStylesEx.WS_EX_TOOLWINDOW
            | User32.WindowStylesEx.WS_EX_TRANSPARENT);
        _ = User32.SetWindowLong(hWnd, User32.WindowLongFlags.GWL_EXSTYLE, style);
    }

    protected override void OnClosed(EventArgs e)
    {
        _hideCancellation?.Cancel();
        _hideCancellation?.Dispose();
        _hideCancellation = null;
        _instance = null;

        base.OnClosed(e);
    }

    private void ShowStatusCore(bool isEnabled)
    {
        _hideCancellation?.Cancel();
        _hideCancellation?.Dispose();
        _hideCancellation = new CancellationTokenSource();

        UpdateStatus(isEnabled);

        bool wasVisible = IsVisible;
        StopWindowAnimations();

        if (!wasVisible)
        {
            Opacity = 0;
            ContentTranslate.Y = 16;
            Show();
        }
        else
        {
            Opacity = 1;
            ContentTranslate.Y = 0;
        }

        PositionOnActiveScreen();
        AnimateIn();
        _ = HideAfterDelayAsync(_hideCancellation.Token);
    }

    private void UpdateStatus(bool isEnabled)
    {
        StatusText.Text = isEnabled ? "自动连点 已开启" : "自动连点 已关闭";

        var easing = new QuadraticEase { EasingMode = EasingMode.EaseOut };
        var duration = TimeSpan.FromMilliseconds(AnimationDurationMilliseconds);

        StatusIndicator.BeginAnimation(WidthProperty, new DoubleAnimation
        {
            To = isEnabled ? 60 : 36,
            Duration = duration,
            EasingFunction = easing
        });
        StatusIndicator.BeginAnimation(OpacityProperty, new DoubleAnimation
        {
            To = isEnabled ? 1 : 0.2,
            Duration = duration,
            EasingFunction = easing
        });
        ShackleRotation.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, new DoubleAnimation
        {
            To = isEnabled ? 0 : 25,
            Duration = duration,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });
    }

    private void PositionOnActiveScreen()
    {
        nint hWnd = new WindowInteropHelper(this).Handle;
        nint foregroundWindow = (nint)User32.GetForegroundWindow();
        Screen screen = foregroundWindow != IntPtr.Zero
            ? Screen.FromHandle(foregroundWindow)
            : Screen.PrimaryScreen!;

        User32.SetWindowPosFlags flags = User32.SetWindowPosFlags.SWP_NOSIZE
            | User32.SetWindowPosFlags.SWP_NOACTIVATE
            | User32.SetWindowPosFlags.SWP_SHOWWINDOW;

        // Move to the target display first so WPF applies that display's DPI.
        _ = User32.SetWindowPos(
            hWnd,
            User32.SpecialWindowHandles.HWND_TOPMOST,
            screen.WorkingArea.Left,
            screen.WorkingArea.Top,
            0,
            0,
            flags);

        UpdateLayout();
        _ = User32.GetWindowRect(hWnd, out RECT windowRect);

        int left = screen.WorkingArea.Left + (screen.WorkingArea.Width - windowRect.Width) / 2;
        int top = screen.WorkingArea.Bottom - windowRect.Height - 16;

        _ = User32.SetWindowPos(
            hWnd,
            User32.SpecialWindowHandles.HWND_TOPMOST,
            left,
            top,
            0,
            0,
            flags);
    }

    private void AnimateIn()
    {
        var duration = TimeSpan.FromMilliseconds(AnimationDurationMilliseconds);
        var easing = new CubicEase { EasingMode = EasingMode.EaseOut };

        BeginAnimation(OpacityProperty, new DoubleAnimation
        {
            To = 1,
            Duration = duration,
            EasingFunction = easing
        });
        ContentTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, new DoubleAnimation
        {
            To = 0,
            Duration = duration,
            EasingFunction = easing
        });
    }

    private async Task HideAfterDelayAsync(CancellationToken token)
    {
        try
        {
            await Task.Delay(DisplayDurationMilliseconds, token);

            var duration = TimeSpan.FromMilliseconds(AnimationDurationMilliseconds);
            var easing = new CubicEase { EasingMode = EasingMode.EaseIn };

            BeginAnimation(OpacityProperty, new DoubleAnimation
            {
                To = 0,
                Duration = duration,
                EasingFunction = easing
            });
            ContentTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, new DoubleAnimation
            {
                To = 16,
                Duration = duration,
                EasingFunction = easing
            });

            await Task.Delay(AnimationDurationMilliseconds, token);
            Hide();
            StopWindowAnimations();
            Opacity = 0;
            ContentTranslate.Y = 16;
        }
        catch (OperationCanceledException)
        {
            // A newer key press restarts the display timer.
        }
    }

    private void StopWindowAnimations()
    {
        BeginAnimation(OpacityProperty, null);
        ContentTranslate.BeginAnimation(System.Windows.Media.TranslateTransform.YProperty, null);
    }
}
