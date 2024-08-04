using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Vanara.PInvoke;

namespace Relauncher.Views.Controls;

public static class WindowBackdrop
{
    public static bool ApplyBackdrop(Window window)
    {
        if (window is null)
        {
            return false;
        }

        if (window.IsLoaded)
        {
            nint windowHandle = new WindowInteropHelper(window).Handle;

            if (windowHandle == 0x00)
            {
                return false;
            }

            return ApplyBackdrop(windowHandle);
        }

        window.Loaded += (sender, _) =>
        {
            nint windowHandle =
                new WindowInteropHelper(sender as Window ?? null)?.Handle
                ?? IntPtr.Zero;

            if (windowHandle == 0x00)
                return;

            ApplyBackdrop(windowHandle);
        };

        return true;
    }

    public static bool ApplyBackdrop(nint hWnd)
    {
        if (hWnd == 0x00 || !User32.IsWindow(hWnd))
        {
            return false;
        }

        _ = DwmApi.DwmSetWindowAttribute(
            hWnd,
            DwmApi.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
            0x1,
            Marshal.SizeOf<int>()
        );

        UxTheme.WTA_OPTIONS wtaOptions = new()
        {
            Flags = UxTheme.WTNCA.WTNCA_NODRAWCAPTION,
            Mask = (uint)UxTheme.ThemeDialogTextureFlags.ETDT_VALIDBITS,
        };

        _ = UxTheme.SetWindowThemeAttribute(
            hWnd,
            UxTheme.WINDOWTHEMEATTRIBUTETYPE.WTA_NONCLIENT,
            wtaOptions,
            (uint)Marshal.SizeOf<UxTheme.WTA_OPTIONS>()
        );

        HRESULT dwmApiResult = Interop.DwmSetWindowAttribute(
            (HWND)hWnd,
            DwmApi.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
            (int)DwmApi.DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW,
            Marshal.SizeOf<int>()
        );

        return dwmApiResult == HRESULT.S_OK;
    }
}
