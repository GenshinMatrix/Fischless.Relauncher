using Relauncher.Extensions;
using Relauncher.Helper;
using Relauncher.Models;
using Relauncher.Relaunchs;
using Relauncher.Views;
using Relauncher.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Vanara.PInvoke;
using Application = System.Windows.Application;

namespace Relauncher.Core.Relaunchs;

public static class GenshinRelaunching
{
    public static async Task StartAsync(GenshinLauncherOption? option = null, CancellationToken? token = null)
    {
        MaskWindow maskWin = MaskWindow.CreateWindow();
        FpsWindow fpsWin = FpsWindow.CreateWindow();

        await Task.Delay(500);
        await Task.Run(async () =>
        {
            var tokenSource = new CancellationTokenSource();

            while (!(token?.IsCancellationRequested ?? false))
            {
                try
                {
                    // HYP: Inject UI
                    {
                        using Process? process = Process.GetProcessesByName("HYP").FirstOrDefault();

                        if (process != null)
                        {
                            nint hWnd = maskWin.Handle;
                            nint targetHWnd = process.MainWindowHandle;

                            if (hWnd == IntPtr.Zero)
                            {
                                maskWin = MaskWindow.CreateWindow();
                                hWnd = maskWin.Handle;
                            }

                            maskWin.Dispatcher.Invoke(() =>
                            {
                                User32.WINDOWPLACEMENT placement = default;
                                _ = User32.GetWindowPlacement(targetHWnd, ref placement);

                                if (placement.showCmd != ShowWindowCommand.SW_HIDE)
                                {
                                    if (User32.GetParent(hWnd) != targetHWnd)
                                    {
                                        _ = User32.SetParent(hWnd, targetHWnd);
                                    }
                                    maskWin.Visibility = Visibility.Visible;
                                    _ = User32.GetClientRect(targetHWnd, out RECT rect);
                                    _ = User32.SetWindowPos(hWnd, IntPtr.Zero, rect.Width - (int)DpiHelper.CalcDPI(176f), (int)DpiHelper.CalcDPI(9f), 100, 100, User32.SetWindowPosFlags.SWP_SHOWWINDOW);

                                    if (!Application.Current.Windows.OfType<GenshinSettingsWindow>().Any())
                                    {
                                        _ = User32.EnableWindow(targetHWnd, true);
                                    }
                                    else
                                    {
                                        foreach (var win in Application.Current.Windows.OfType<GenshinSettingsWindow>())
                                        {
                                            _ = User32.SetWindowPos(targetHWnd, win.Handle, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_NOSIZE | User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOACTIVATE | User32.SetWindowPosFlags.SWP_SHOWWINDOW);
                                        }
                                    }
                                }
                                else
                                {
                                    maskWin.Close();
                                }
                            });
                        }
                        else
                        {
                            maskWin.Dispatcher.Invoke(() =>
                            {
                                maskWin.Visibility = Visibility.Collapsed;
                            });
                        }
                    }

                    // HYP: Relaunch Genshin
                    {
                        using Process? process = Process.GetProcessesByName("Yuanshen").FirstOrDefault();

                        if (process != null)
                        {
                            int? parentProcessId = Interop.GetParentProcessId(process.Id);

                            if (parentProcessId != null)
                            {
                                try
                                {
                                    using Process? parentProcess = Process.GetProcessesByName("HYP").FirstOrDefault();

                                    if (parentProcess != null)
                                    {
                                        if (parentProcess.Id == parentProcessId.Value)
                                        {
                                            string? exeName = Interop.GetExeNameByProcessId((uint)parentProcess.Id);
                                            GenshinConfigurations config = Configurations.Genshin.Get();

                                            GenshinLauncherOption option = new()
                                            {
                                                GamePath = exeName,
                                                WorkingDirectory = Path.GetDirectoryName(exeName),
                                                Account = null!, // TODO: Not supported
                                                Arguments = new GenshinArgumentsOption()
                                                {
                                                    IsUseArguments = config.IsUseArguments,
                                                    IsUseWindowModeExclusive = config.IsUseWindowModeExclusive,
                                                    IsScreenFullscreen = config.IsScreenFullscreen,
                                                    IsPopupwindow = config.IsPopupwindow,
                                                    IsPlatformTypeCloudThirdPartyMobile = config.IsPlatformTypeCloudThirdPartyMobile,
                                                    IsScreenWidth = config.IsScreenWidth,
                                                    ScreenWidth = config.ScreenWidth,
                                                    IsScreenHeight = config.IsScreenHeight,
                                                    ScreenHeight = config.ScreenHeight,
                                                    IsMonitor = config.IsMonitor,
                                                    Monitor = config.Monitor,
                                                },
                                                Advance = new GenshinAdvanceOption()
                                                {
                                                    IsDisnetLaunching = config.IsDisnetLaunching,
                                                    IsUseHDR = config.IsUseHDR,
                                                },
                                                Linkage = new GenshinLinkageOption()
                                                {
                                                    ReShadePath = config.ReShadePath,
                                                    IsUseReShade = config.IsUseReShade,
                                                    IsUseReShadeSilent = config.IsUseReShadeSilent,
                                                    IsUseBetterGI = config.IsUseBetterGI,
                                                },
                                                Region = null,
                                                Unlocker = new GenshinUnlockerOption()
                                                {
                                                    IsUnlockFps = config.IsUnlockFps,
                                                    UnlockFps = config.UnlockFps,
                                                    UnlockFpsMethod = config.UnlockFpsMethod,
                                                },
                                            };

                                            process.Kill();
                                            await GenshinLauncher.LaunchAsync(delayMs: 1000, relaunchMethod: GenshinRelaunchMethod.Kill, option: option);
                                        }
                                    }
                                }
                                catch
                                {
                                    ///
                                }
                            }
                        }
                    }

                    // Genshin: Inject UI
                    {
                        bool isFpsShouldShown = false;

                        if (Configurations.Genshin.Get().IsShowFps)
                        {
                            using Process? process = Process.GetProcessesByName("Yuanshen").FirstOrDefault();

                            if (process != null)
                            {
                                nint hWnd = fpsWin.Handle;
                                nint targetHWnd = process.MainWindowHandle;

                                if (hWnd == IntPtr.Zero)
                                {
                                    fpsWin = FpsWindow.CreateWindow();
                                    hWnd = fpsWin.Handle;
                                }

                                fpsWin.Dispatcher.Invoke(() =>
                                {
                                    fpsWin.Pid = (uint)process.Id;
                                    fpsWin.Visibility = Visibility.Visible;
                                    if (User32.GetParent(hWnd) != targetHWnd)
                                    {
                                        _ = User32.SetParent(hWnd, targetHWnd);
                                    }
                                    _ = User32.GetClientRect(targetHWnd, out RECT rect);
                                    float x = DpiHelper.GetScale(targetHWnd).X;
                                    _ = User32.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, (int)(rect.Width * x), (int)(rect.Height * x), User32.SetWindowPosFlags.SWP_SHOWWINDOW);
                                });

                                isFpsShouldShown = true;
                            }
                            else
                            {
                                isFpsShouldShown = false;
                            }
                        }
                        else
                        {
                            isFpsShouldShown = false;
                        }

                        if (!isFpsShouldShown)
                        {
                            fpsWin.Dispatcher.Invoke(() =>
                            {
                                fpsWin.Pid = 0;
                                fpsWin.Visibility = Visibility.Collapsed;
                            });
                        }
                    }

                    // Genshin: Dark mode
                    {
                        using Process? process = Process.GetProcessesByName("Yuanshen").FirstOrDefault();

                        if (process != null && process.MainWindowHandle != IntPtr.Zero)
                        {
                            _ = WindowBackdropExtension.SetRoundedCorners(process.MainWindowHandle, !Configurations.Genshin.Get().IsSquareCorner);
                            _ = WindowBackdropExtension.ApplyWindowDarkMode(process.MainWindowHandle, Configurations.Genshin.Get().IsDarkMode);
                        }
                    }

                    // Genshin: Auto mute
                    {
                        // TODO
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                Thread.Sleep(200);
            }
        });
    }
}
