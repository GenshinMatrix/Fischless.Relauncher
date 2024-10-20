using System.Reflection;

namespace Fischless.Relauncher;

internal static class AppConfig
{
    public static string PackName => "miHoYo-reLauncher";
    public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version!;
    public static string VersionString => $"v{Version.ToString(3)}";
    public static string LogFolder { get; internal set; } = null!;
}
