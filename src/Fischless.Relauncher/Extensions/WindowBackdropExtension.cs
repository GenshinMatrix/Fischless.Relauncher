using Fischless.Relauncher.Win32;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace Fischless.Relauncher.Extensions;

internal static class WindowBackdropExtension
{
    public static bool SetRoundedCorners(nint hWnd, bool enable = true)
    {
        if (hWnd == IntPtr.Zero)
        {
            return false;
        }

        if (!User32.IsWindow(hWnd))
        {
            return false;
        }

        if (Environment.OSVersion.Version.Build < 22000)
        {
            return false;
        }

        HRESULT result = Interop.DwmGetWindowAttribute(hWnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, out int sourcePerf, Marshal.SizeOf(typeof(int)));

        int targetPerf = enable ? (int)DwmApi.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND : (int)DwmApi.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND;

        if (result == HRESULT.S_OK)
        {
            if (sourcePerf == targetPerf)
            {
                return true;
            }
        }

        result = Interop.DwmSetWindowAttribute(hWnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, targetPerf, Marshal.SizeOf(typeof(int)));
        return result == HRESULT.S_OK;
    }

    public static bool ApplyWindowDarkMode(nint hWnd, bool isDarkMode)
    {
        if (hWnd == IntPtr.Zero)
        {
            return false;
        }

        if (!User32.IsWindow(hWnd))
        {
            return false;
        }

        if (Environment.OSVersion.Version.Build < 17763)
        {
            return false;
        }

        HRESULT result = Interop.DwmGetWindowAttribute(hWnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, out int isDarkModeEnabled, Marshal.SizeOf(typeof(int)));

        if (result == HRESULT.S_OK)
        {
            if (isDarkMode && isDarkModeEnabled != 0)
            {
                return true;
            }
            else if (!isDarkMode && isDarkModeEnabled == 0)
            {
                return true;
            }
        }

        if (Environment.OSVersion.Version.Build >= 22523)
        {
            result = Interop.DwmSetWindowAttribute(hWnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, isDarkMode ? 1 : 0, Marshal.SizeOf(typeof(int)));
            return result == HRESULT.S_OK;
        }
        else
        {
            const DwmApi.DWMWINDOWATTRIBUTE DMWA_USE_IMMERSIVE_DARK_MODE_OLD = DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE - 1;
            result = Interop.DwmSetWindowAttribute(hWnd, DMWA_USE_IMMERSIVE_DARK_MODE_OLD, isDarkMode ? 1 : 0, Marshal.SizeOf(typeof(int)));
            return result == HRESULT.S_OK;
        }
    }
}
