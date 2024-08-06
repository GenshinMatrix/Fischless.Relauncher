using Relauncher.Extensions;
using Relauncher.Views;
using System.Diagnostics;
using Application = System.Windows.Application;
using NotifyIcon = NotifyIconEx.NotifyIcon;

namespace Relauncher;

internal static class TrayIconManager
{
    public static void Setup()
    {
        var notifyIcon = new NotifyIcon()
        {
            Text = "miHoYo-reLauncher",
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule?.FileName!)!,
        };
        notifyIcon.AddMenu("打开设置 (&S)", (_, _) =>
        {
            Application.Current.Windows.OfType<GenshinSettingsWindow>().ToList().ForEach(x => x.Close());
            new GenshinSettingsWindow() { WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen }.Show();
        });
        notifyIcon.AddMenu("-");
        notifyIcon.AddMenu("重启 (&R)", (_, _) => RuntimeHelper.Restart(forced: true));
        notifyIcon.AddMenu("退出 (&E)", (_, _) => Application.Current.Shutdown());
    }
}
