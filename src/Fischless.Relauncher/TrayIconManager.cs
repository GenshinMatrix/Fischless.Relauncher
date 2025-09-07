using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Fischless.Configuration;
using Fischless.Relauncher.Core.Relaunchs;
using Fischless.Relauncher.Extensions;
using Fischless.Relauncher.Models;
using Fischless.Relauncher.Models.Messages;
using Fischless.Relauncher.Views;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Vanara.PInvoke;
using Wpf.Ui.Violeta.Win32;

namespace Fischless.Relauncher;

internal class TrayIconManager : IDisposable
{
    private static TrayIconManager _instance = null!;

    private readonly TrayIconHost _icon = null!;

    private readonly TrayMenuItem _itemAutoRun = null!;
    private readonly TrayMenuItem _itemAutoMute = null!;

    private TrayIconManager()
    {
        _icon = new TrayIconHost()
        {
            ToolTipText = "Fischless.Relauncher",
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule?.FileName!)!.Handle,
            Menu =
            [
                new TrayMenuItem()
                {
                    Header = $"v{Assembly.GetExecutingAssembly().GetName().Version!.ToString(3)}",
                    IsEnabled = false,
                },
                new TraySeparator(),
                new TrayMenuItem()
                {
                    Header = "启动游戏 (&L)",
                    Command = new RelayCommand(async () =>
                    {
                        if (await GenshinLauncher.TryGetProcessAsync())
                        {
                            if (await MessageBox.QuestionAsync("检测到游戏已启动，是否重新启动游戏？") != MessageBoxResult.Yes)
                            {
                                return;
                            }
                        }

                        await GenshinLauncher.LaunchAsync(delayMs: 1000, relaunchMethod: GenshinRelaunchMethod.Kill, option: GenshinLauncherOptionProvider.GetOption());
                    }),
                },
                new TrayMenuItem()
                {
                    Header = "退出游戏 (&K)",
                    Command = new RelayCommand(async () =>
                    {
                        _ = await GenshinLauncher.TryGetProcessAsync(async p =>
                        {
                            await Task.CompletedTask;
                            p?.Kill();
                        });
                    }),
                },
                new TraySeparator(),
                new TrayMenuItem()
                {
                    Header = "打开设置 (&O)",
                    Command = new RelayCommand(() =>
                    {
                        Application.Current.Windows.OfType<GenshinSettingsWindow>().ToList().ForEach(x => x.Close());
                        new GenshinSettingsWindow()
                        {
                            Topmost = false,
                            ShowInTaskbar = true,
                            WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        }.Show();
                    }),
                },
                _itemAutoMute = new TrayMenuItem()
                {
                    Header = "自动静音 (&M)",
                    Command = new RelayCommand(() =>
                    {
                        var config = Configurations.Genshin.Get();
                        GenshinMuter.IsEnabled = config.IsUseAutoMute = !_itemAutoMute.IsChecked;
                        Configurations.Genshin.Set(config);
                        ConfigurationManager.Save();
                        WeakReferenceMessenger.Default.Send(new GenshinAutoMuteChangedMessage());
                    }),
                },
                new TraySeparator(),
                _itemAutoRun = new TrayMenuItem()
                {
                    Header = "启动时自动运行 (&S)",
                    Command = new RelayCommand(() =>
                    {
                        if (AutoStartupHelper.IsAutorun())
                            AutoStartupHelper.RemoveAutorunShortcut();
                        else
                            AutoStartupHelper.CreateAutorunShortcut();
                    }),
                },
                new TrayMenuItem()
                {
                    Header = "重启 (&R)",
                    Command = new RelayCommand(() => RuntimeHelper.Restart(forced: true)),
                },
                new TrayMenuItem()
                {
                    Header = "退出 (&E)",
                    Command = new RelayCommand(Application.Current.Shutdown),
                },
            ],
        };

        _icon.RightDown += (_, _) =>
        {
            _itemAutoRun.IsChecked = AutoStartupHelper.IsAutorun();
            _itemAutoMute.IsChecked = Configurations.Genshin.Get().IsUseAutoMute;
        };

        _icon.LeftDoubleClick += async (_, _) =>
        {
            _ = await GenshinLauncher.TryGetProcessAsync(async p =>
            {
                await Task.CompletedTask;

                if (p != null)
                {
                    nint hWnd = p.MainWindowHandle != IntPtr.Zero
                        ? p.MainWindowHandle
                        : GenshinLauncher.TryGetHandleByWindowName();

                    if (User32.IsWindowVisible(hWnd))
                    {
                        _ = User32.ShowWindow(hWnd, ShowWindowCommand.SW_HIDE);
                    }
                    else
                    {
                        _ = User32.ShowWindow(hWnd, ShowWindowCommand.SW_RESTORE);
                        _ = User32.ShowWindow(hWnd, ShowWindowCommand.SW_SHOW);
                        _ = User32.SetActiveWindow(hWnd);
                    }
                }
            });
        };

        GenshinDragMove.IsEnabled = Configurations.Genshin.Get().IsUseBorderless;
        GenshinMuter.IsEnabled = Configurations.Genshin.Get().IsUseAutoMute;

        if (Configurations.Genshin.Get().IsUseAutoClick)
        {
            // Ensure GenshinMonitor is initialized
            _ = GenshinMonitor.Instance.Value;
        }
    }

    public void Dispose()
    {
        _icon.IsVisible = false;
    }

    public static TrayIconManager GetInstance()
    {
        return _instance ??= new TrayIconManager();
    }
}
