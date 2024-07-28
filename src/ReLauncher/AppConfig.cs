using System.Reflection;

namespace Relauncher;

internal static class AppConfig
{
    public static string PackName => "miHoYo-reLauncher";
    public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version!;
    public static string VersionString => $"v{Version.Major}.{Version.Minor}.{Version.Build}";
    public static string LogFolder { get; internal set; } = null!;
}
