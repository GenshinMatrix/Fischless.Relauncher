using System.Runtime.InteropServices;
using Vanara.PInvoke;
using Wpf.Ui.Interop;

namespace Relauncher.Extensions;

internal static class WindowBackdropExtension
{
    public enum DwmWindowCornerPreference : uint
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(nint hwnd, DwmApi.DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

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

        // TODO: Check if the window is already rounded
        int preference = enable ? (int)DwmWindowCornerPreference.DWMWCP_ROUND : (int)DwmWindowCornerPreference.DWMWCP_DONOTROUND;
        int hr = DwmSetWindowAttribute(hWnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref preference, sizeof(int));
        return hr >= 0;
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

        // TODO: Remove unsafe
        unsafe
        {
            DwmApi.DWMWINDOWATTRIBUTE attribute = DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE;
            int isDarkModeEnabled;
            nint isDarkModeEnabledPtr = (nint)(&isDarkModeEnabled);
            var result = DwmApi.DwmGetWindowAttribute(hWnd, attribute, isDarkModeEnabledPtr, Marshal.SizeOf(typeof(int)));

            if (result == 0)
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
        }

        if (isDarkMode)
        {
            // TODO: Inlined method
            UnsafeNativeMethods.ApplyWindowDarkMode(hWnd);
        }
        else
        {
            // TODO: Inlined method
            UnsafeNativeMethods.RemoveWindowDarkMode(hWnd);
        }

        return true;
    }
}
