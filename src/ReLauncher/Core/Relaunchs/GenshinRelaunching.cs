using Relauncher.Helper;
using Relauncher.Relaunchs;
using Relauncher.Views;
using System.Diagnostics;
using System.Windows;
using Vanara.PInvoke;
using Application = System.Windows.Application;

namespace Relauncher.Core.Relaunchs;

public static class GenshinRelaunching
{
    public static async Task StartAsync(GenshinLauncherOption? option = null, CancellationToken? token = null)
    {
        MaskWindow window = MaskWindow.CreateWindow();

        while (!(token?.IsCancellationRequested ?? false))
        {
            try
            {
                ///
                {
                    Process? process = Process.GetProcessesByName("HYP").FirstOrDefault();

                    if (process != null)
                    {
                        nint hWnd = window.Handle;
                        nint targetHWnd = process.MainWindowHandle;

                        if (hWnd == IntPtr.Zero)
                        {
                            window = MaskWindow.CreateWindow();
                            hWnd = window.Handle;
                        }

                        window.Dispatcher.Invoke(() =>
                        {
                            window.Visibility = Visibility.Visible;
                            if (User32.GetParent(hWnd) != targetHWnd)
                            {
                                _ = User32.SetParent(hWnd, targetHWnd);
                            }
                            _ = User32.GetClientRect(targetHWnd, out RECT rect);
                            ;
                            _ = User32.SetWindowPos(hWnd, IntPtr.Zero, rect.Width - (int)DpiHelper.CalcDPI(176f), (int)DpiHelper.CalcDPI(9f), 100, 100, User32.SetWindowPosFlags.SWP_SHOWWINDOW);

                            if (!Application.Current.Windows.OfType<GenshinSettingsWindow>().Any())
                            {
                                _ = User32.EnableWindow(targetHWnd, true);
                            }
                        });
                    }
                }

                ///
                {
                    Process? process = Process.GetProcessesByName("Yuanshen").FirstOrDefault();

                    if (process != null)
                    {
                        int? parentProcessId = Interop.GetParentProcessId(process.Id);

                        if (parentProcessId != null)
                        {
                            Process? parentProcess = Process.GetProcessById(parentProcessId.Value);

                            if (parentProcess.ProcessName == "HYP")
                            {
                                process.Kill();
                                await GenshinLauncher.KillAsync(GenshinRelaunchMethod.Kill);
                                await GenshinLauncher.LaunchAsync(delayMs: 1000, relaunchMethod: GenshinRelaunchMethod.Kill);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            Thread.Sleep(200);
        }
    }
}
