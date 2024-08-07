using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Vanara.PInvoke;
using Wpf.Ui.Controls;

namespace Relauncher.Views;

[ObservableObject]
public partial class IdentifyMonitorWindow : Window
{
    [ObservableProperty]
    private string monitor;

    public IdentifyMonitorWindow(MonitorInfo displayArea, int index)
    {
        DataContext = this;
        InitializeComponent();
        Monitor = $"{displayArea.DeviceName}:{index}";

        Width = displayArea.MonitorArea.Width / displayArea.ScaleX * 0.13d;
        Height = displayArea.MonitorArea.Height / displayArea.ScaleY * 0.13d;
        Top = (displayArea.MonitorArea.Top + 32) / displayArea.ScaleY;
        Left = (displayArea.MonitorArea.Left + 40) / displayArea.ScaleX ;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        if (WindowBackdrop.IsSupported(WindowBackdropType.Mica))
        {
            WindowBackdrop.ApplyBackdrop(this, WindowBackdropType.Mica);
        }
    }

    public static async ValueTask IdentifyAllMonitorsAsync(int secondsDelay)
    {
        List<IdentifyMonitorWindow> windows = [];

        IReadOnlyList<MonitorInfo> displayAreas = GetAllMonitors();
        for (int i = 0; i < displayAreas.Count; i++)
        {
            windows.Add(new IdentifyMonitorWindow(displayAreas[i], i + 1));
        }

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.Show();
        }

        await Task.Delay(TimeSpan.FromSeconds(secondsDelay)).ConfigureAwait(true);

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.Close();
        }
    }

    public class MonitorInfo
    {
        public string? DeviceName { get; set; }
        public RECT MonitorArea { get; set; }
        public RECT WorkArea { get; set; }
        public bool IsPrimary { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
    }

    public static List<MonitorInfo> GetAllMonitors()
    {
        List<MonitorInfo> monitors = [];

        _ = User32.EnumDisplayMonitors(IntPtr.Zero, null, callback, IntPtr.Zero);
        return monitors;

        bool callback(nint hMonitor, nint hdcMonitor, PRECT lprcMonitor, nint dwData)
        {
            User32.MONITORINFOEX mi = new();
            mi.cbSize = (uint)Marshal.SizeOf(mi);
            if (User32.GetMonitorInfo(hMonitor, ref mi))
            {
                _ = SHCore.GetDpiForMonitor(hMonitor, SHCore.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out uint dpiX, out uint dpiY);

                monitors.Add(new MonitorInfo()
                {
                    DeviceName = mi.szDevice,
                    MonitorArea = mi.rcMonitor,
                    WorkArea = mi.rcWork,
                    IsPrimary = (mi.dwFlags & User32.MonitorInfoFlags.MONITORINFOF_PRIMARY) != 0,
                    ScaleX = dpiX / 96d,
                    ScaleY = dpiY / 96d,
                });
            }
            return true;
        }
    }

    private static void PrintDisplayConfigInfo()
    {
        if (GetDisplayConfigInfo(out Gdi32.DISPLAYCONFIG_PATH_INFO[] paths, out Gdi32.DISPLAYCONFIG_MODE_INFO[] modes))
        {
            foreach (var path in paths)
            {
                nint targetDeviceNamePtr = Marshal.AllocHGlobal(Marshal.SizeOf<Gdi32.DISPLAYCONFIG_TARGET_DEVICE_NAME>());

                try
                {
                    var targetDeviceName = new Gdi32.DISPLAYCONFIG_TARGET_DEVICE_NAME()
                    {
                        header = new Gdi32.DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                            type = Gdi32.DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME,
                            size = (uint)Marshal.SizeOf<Gdi32.DISPLAYCONFIG_TARGET_DEVICE_NAME>()
                        }
                    };
                    Marshal.StructureToPtr(targetDeviceName, targetDeviceNamePtr, false);
                    var status = User32.DisplayConfigGetDeviceInfo(targetDeviceNamePtr);

                    if (status == Win32Error.ERROR_SUCCESS)
                    {
                        Debug.WriteLine($"Device Name: {targetDeviceName.monitorFriendlyDeviceName}");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to get device info. Error: {status}");
                    }
                }
                catch
                {
                    Marshal.FreeHGlobal(targetDeviceNamePtr);
                }

                nint targetBaseTypePtr = Marshal.AllocHGlobal(Marshal.SizeOf<Gdi32.DISPLAYCONFIG_TARGET_BASE_TYPE>());

                try
                {
                    var targetBaseType = new Gdi32.DISPLAYCONFIG_TARGET_BASE_TYPE()
                    {
                        header = new Gdi32.DISPLAYCONFIG_DEVICE_INFO_HEADER
                        {
                            adapterId = path.targetInfo.adapterId,
                            id = path.targetInfo.id,
                            type = Gdi32.DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE,
                            size = (uint)Marshal.SizeOf<Gdi32.DISPLAYCONFIG_TARGET_BASE_TYPE>()
                        }
                    };

                    Marshal.StructureToPtr(targetBaseType, targetBaseTypePtr, false);
                    var status = User32.DisplayConfigGetDeviceInfo(targetBaseTypePtr);

                    if (status == Win32Error.ERROR_SUCCESS)
                    {
                        Debug.WriteLine($"Monitor Type: {targetBaseType.baseOutputTechnology}");
                    }
                    else
                    {
                        Debug.WriteLine($"Failed to get monitor type. Error: {status}");
                    }
                }
                catch
                {
                    Marshal.FreeHGlobal(targetBaseTypePtr);
                }
            }
        }
        else
        {
            Debug.WriteLine("Failed to get display configuration information.");
        }
    }

    private static bool GetDisplayConfigInfo(out Gdi32.DISPLAYCONFIG_PATH_INFO[] paths, out Gdi32.DISPLAYCONFIG_MODE_INFO[] modes)
    {
        var status = User32.GetDisplayConfigBufferSizes(User32.QDC.QDC_ONLY_ACTIVE_PATHS, out uint pathCount, out uint modeCount);

        if (status != Win32Error.ERROR_SUCCESS)
        {
            Debug.WriteLine($"Failed to get buffer sizes. Error: {status}");
            paths = null!;
            modes = null!;
            return false;
        }

        paths = new Gdi32.DISPLAYCONFIG_PATH_INFO[pathCount];
        modes = new Gdi32.DISPLAYCONFIG_MODE_INFO[modeCount];

        status = User32.QueryDisplayConfig(User32.QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);

        if (status != Win32Error.ERROR_SUCCESS)
        {
            Debug.WriteLine($"Failed to query display config. Error: {status}");
            return false;
        }

        return true;
    }
}
