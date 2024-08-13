using CommunityToolkit.Mvvm.Messaging;
using Relauncher.Core.Configs;
using Relauncher.Core.Relaunchs;
using Relauncher.Extensions;
using Relauncher.Models;
using Relauncher.Models.Messages;
using Relauncher.Views;
using System.Diagnostics;
using System.Reflection;
using Application = System.Windows.Application;
using NotifyIcon = NotifyIconEx.NotifyIcon;

namespace Relauncher;

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
                GenshinMuter.AutoMute = config.IsUseAutoMute = _itemAutoMute!.Checked;
                Configurations.Genshin.Set(config);
                ConfigurationManager.Save();
                WeakReferenceMessenger.Default.Send(new AutoMuteChangedMessage());
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

        GenshinMuter.AutoMute = Configurations.Genshin.Get().IsUseAutoMute;
    }

    public static TrayIconManager GetInstance()
    {
        return _instance ??= new TrayIconManager();
    }
}
