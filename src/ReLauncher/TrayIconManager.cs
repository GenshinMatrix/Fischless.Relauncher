using Relauncher.Extensions;
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

    private readonly ToolStripMenuItem _itemAutorun = null!;

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
        _itemAutorun = (_icon.AddMenu("启动时自动运行 (&S)",
            (_, _) =>
            {
                if (AutoStartupHelper.IsAutorun())
                    AutoStartupHelper.RemoveAutorunShortcut();
                else
                    AutoStartupHelper.CreateAutorunShortcut();
            }) as ToolStripMenuItem)!;
        _icon.AddMenu("重启 (&R)", (_, _) => RuntimeHelper.Restart(forced: true));
        _icon.AddMenu("退出 (&E)", (_, _) => Application.Current.Shutdown());

        _icon.ContextMenuStrip.Opened += (_, _) => _itemAutorun.Checked = AutoStartupHelper.IsAutorun();
    }

    public static TrayIconManager GetInstance()
    {
        return _instance ??= new TrayIconManager();
    }
}
