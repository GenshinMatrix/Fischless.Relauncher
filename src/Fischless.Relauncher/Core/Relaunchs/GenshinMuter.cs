using NAudio.CoreAudioApi;
using Fischless.Relauncher.Core.Loggers;
using System.Diagnostics;
using System.Text;
using Vanara.PInvoke;
using MMDevices = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace Fischless.Relauncher.Core.Relaunchs;

internal static class GenshinMuter
{
    private static bool isEnabled = false;

    public static bool IsEnabled
    {
        get => isEnabled;
        set
        {
            isEnabled = value;
            _ = MuteGameAsync(value);
            if (isEnabled)
            {
                ForegroundWindowHelper.Initialize();
            }
            else
            {
                ForegroundWindowHelper.Uninitialize();
            }
        }
    }

    static GenshinMuter()
    {
        ForegroundWindowHelper.ForegroundWindowChanged += OnForegroundWindowChanged;
    }

#pragma warning disable CS0465

    // TODO: fix CS0465
    public static void Finalize()
    {
        ForegroundWindowHelper.ForegroundWindowChanged -= OnForegroundWindowChanged;
        ForegroundWindowHelper.Uninitialize();
    }

#pragma warning restore CS0465

    private static async void OnForegroundWindowChanged(ForegroundWindowHelperEventArgs e)
    {
        if (!IsEnabled)
        {
            return;
        }

        bool matchProcess = false;
        if (e.HWnd != IntPtr.Zero)
        {
            if (e.WindowTitle == GenshinRegistryKeys.CN || e.WindowTitle == GenshinRegistryKeys.OVERSEA)
            {
                matchProcess = true;
            }
        }
        await MuteGameAsync(!matchProcess);
    }

    public static async Task MuteGameAsync(bool isMuted)
    {
        if (GenshinLauncher.TryGetProcess(out Process? process))
        {
            await MuteProcessAsync(process!.Id, isMuted);
        };
    }

    private static async Task MuteProcessAsync(int pid, bool isMuted)
    {
        try
        {
            await Task.Run(() =>
            {
                MuteProcess(pid, isMuted);
            });
        }
        catch (Exception e)
        {
            Log.Warning(e.ToString());
        }
    }

    private static void MuteProcess(int pid, bool isMuted)
    {
        MMDevices audio = new();
        foreach (MMDevice device in audio.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            for (int i = default; i < device.AudioSessionManager.Sessions.Count; i++)
            {
                AudioSessionControl session = device.AudioSessionManager.Sessions[i];

                if (session.GetProcessID == pid)
                {
                    session.SimpleAudioVolume.Mute = isMuted;
                    break;
                }
            }
        }
    }
}

file static class ForegroundWindowHelper
{
    private static bool isInitialized = false;
    private static User32.HWINEVENTHOOK hook;
    private static User32.WinEventProc? eventProc;

    public static bool Unsupported { get; private set; } = false;

    public static void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;

        eventProc = new User32.WinEventProc(EventProc);

        try
        {
            hook = User32.SetWinEventHook(
                User32.EventConstants.EVENT_SYSTEM_FOREGROUND,
                User32.EventConstants.EVENT_SYSTEM_FOREGROUND,
                HINSTANCE.NULL,
                eventProc,
                0,
                0,
                User32.WINEVENT.WINEVENT_OUTOFCONTEXT | User32.WINEVENT.WINEVENT_SKIPOWNPROCESS);
        }
        catch
        {
            ///
        }

        if (hook.IsNull)
        {
            Unsupported = true;
        }
    }

    public static void Uninitialize()
    {
        if (!isInitialized)
        {
            return;
        }

        isInitialized = false;

        try
        {
            User32.UnhookWinEvent(hook);
            hook = default;
        }
        catch
        {
            ///
        }
    }

    private static void EventProc(User32.HWINEVENTHOOK hWinEventHook, uint winEvent, HWND hWnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        try
        {
            ForegroundWindowChanged?.Invoke(new ForegroundWindowHelperEventArgs(hWnd.DangerousGetHandle()));
        }
        catch
        {
            ///
        }
    }

    internal static event ForegroundWindowHelperEventHandler? ForegroundWindowChanged;
}

file delegate void ForegroundWindowHelperEventHandler(ForegroundWindowHelperEventArgs args);

internal struct ForegroundWindowHelperEventArgs
{
    private bool windowTitleFlag;
    private bool windowClassFlag;
    private string? windowTitle;
    private string? windowClassName;

    internal ForegroundWindowHelperEventArgs(nint hWnd)
    {
        windowTitleFlag = false;
        windowClassFlag = false;
        windowTitle = null;
        windowClassName = null;

        HWnd = hWnd;
    }

    public nint HWnd { get; }

    public string WindowTitle
    {
        get
        {
            if (HWnd == IntPtr.Zero)
            {
                return string.Empty;
            }

            if (!windowTitleFlag)
            {
                var sb = new StringBuilder(256);
                _ = User32.GetWindowText(HWnd, sb, sb.Capacity);
                windowTitle = sb.ToString();
                windowTitleFlag = true;
            }

            return windowTitle!;
        }
    }

    public string WindowClassName
    {
        get
        {
            if (HWnd == IntPtr.Zero)
            {
                return string.Empty;
            }

            if (!windowClassFlag)
            {
                var sb = new StringBuilder(256);
                _ = User32.GetClassName(HWnd, sb, sb.Capacity);
                windowClassName = sb.ToString();
                windowClassFlag = true;
            }

            return windowClassName!;
        }
    }

    public bool IsSystemWindow =>
        string.Equals(WindowClassName, "Shell_TrayWnd", StringComparison.OrdinalIgnoreCase)
        || string.Equals(WindowClassName, "Windows.UI.Core.CoreWindow", StringComparison.OrdinalIgnoreCase)
        || WindowClassName?.StartsWith("HwndWrapper[DefaultDomain;;", StringComparison.OrdinalIgnoreCase) == true;

    private static readonly Dictionary<nint, bool> isWindowOfProcessElevatedCache = [];
}
