using Vanara.PInvoke;

namespace Fischless.Relauncher.Core.Relaunchs;

internal static class GenshinClicker
{
    public static bool IsUseAutoClick { get; set; } = false;

    public static void LeftButtonClick(nint hWnd)
    {
        nint p = (16 << 16) | 16;
        User32.PostMessage(hWnd, User32.WindowMessage.WM_LBUTTONDOWN, IntPtr.Zero, p);
        Thread.Sleep(100);
        User32.PostMessage(hWnd, User32.WindowMessage.WM_LBUTTONUP, IntPtr.Zero, p);
    }

    public static void LeftButtonClickBackground(nint hWnd)
    {
        User32.PostMessage(hWnd, User32.WindowMessage.WM_ACTIVATE, 1, 0);
        nint p = (16 << 16) | 16;
        User32.PostMessage(hWnd, User32.WindowMessage.WM_LBUTTONDOWN, IntPtr.Zero, p);
        Thread.Sleep(100);
        User32.PostMessage(hWnd, User32.WindowMessage.WM_LBUTTONUP, IntPtr.Zero, p);
    }
}
