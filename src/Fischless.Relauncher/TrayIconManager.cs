using CommunityToolkit.Mvvm.Messaging;
using Fischless.Relauncher.Core.Configs;
using Fischless.Relauncher.Core.Relaunchs;
using Fischless.Relauncher.Extensions;
using Fischless.Relauncher.Models;
using Fischless.Relauncher.Models.Messages;
using Fischless.Relauncher.Relaunchs;
using Fischless.Relauncher.Views;
using System.Diagnostics;
using System.Reflection;
using Vanara.PInvoke;
using Wpf.Ui.Violeta.Controls;
using Application = System.Windows.Application;
using NotifyIcon = NotifyIconEx.NotifyIcon;

namespace Fischless.Relauncher;

internal class TrayIconManager
{
    private static TrayIconManager _instance = null!;

    private readonly NotifyIcon _icon = null!;

    private readonly ToolStripMenuItem? _itemAutoRun = null;
    private readonly ToolStripMenuItem? _itemAutoMute = null;

    private TrayIconManager()
    {
        _icon = new NotifyIcon()
        {
            Text = "miHoYo-reLauncher",
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule?.FileName!)!,
            Visible = true
        };
        _icon.AddMenu($"v{Assembly.GetExecutingAssembly().GetName().Version!.ToString("A.B.C")}").Enabled = false;
        _icon.AddMenu("-");
        _icon.AddMenu("启动游戏 (&L)", async (_, _) =>
        {
            await GenshinLauncher.LaunchAsync(delayMs: 1000, relaunchMethod: GenshinRelaunchMethod.Kill, option: GenshinLauncherOptionProvider.GetOption());
        });
        _icon.AddMenu("打开设置 (&O)", (_, _) =>
        {
            Application.Current.Windows.OfType<GenshinSettingsWindow>().ToList().ForEach(x => x.Close());
            new GenshinSettingsWindow()
            {
                Topmost = false,
                ShowInTaskbar = true,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
            }.Show();
        });
        (_itemAutoMute = _icon.AddMenu("自动静音 (&M)",
            (_, _) =>
            {
                var config = Configurations.Genshin.Get();
                GenshinMuter.IsEnabled = config.IsUseAutoMute = _itemAutoMute!.Checked;
                Configurations.Genshin.Set(config);
                ConfigurationManager.Save();
                WeakReferenceMessenger.Default.Send(new GenshinAutoMuteChangedMessage());
            }) as ToolStripMenuItem)!.CheckOnClick = true;
        _itemAutoRun = _icon.AddMenu("启动时自动运行 (&S)",
            (_, _) =>
            {
                if (AutoStartupHelper.IsAutorun())
                    AutoStartupHelper.RemoveAutorunShortcut();
                else
                    AutoStartupHelper.CreateAutorunShortcut();
            }) as ToolStripMenuItem;
        _icon.AddMenu("重启 (&R)", (_, _) => RuntimeHelper.Restart(forced: true));
        _icon.AddMenu("退出 (&E)", (_, _) => Application.Current.Shutdown());

        _icon.ContextMenuStrip.Opened += (_, _) =>
        {
            _itemAutoRun!.Checked = AutoStartupHelper.IsAutorun();
            _itemAutoMute!.Checked = Configurations.Genshin.Get().IsUseAutoMute;
        };

        _icon.MouseDoubleClick += async (_, _) =>
        {
            _ = await GenshinLauncher.TryGetProcessAsync(async t =>
            {
                await Task.CompletedTask;

                if (t != null)
                {
                    nint hWnd = t.MainWindowHandle != IntPtr.Zero
                        ? t.MainWindowHandle
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
    }

    public static TrayIconManager GetInstance()
    {
        return _instance ??= new TrayIconManager();
    }
}
