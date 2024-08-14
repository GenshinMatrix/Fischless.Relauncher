namespace Fischless.Relauncher.Extensions;

internal static class VersionExtension
{
    public static string ToString(this Version version, string format)
    {
        if (!string.IsNullOrEmpty(format))
        {
            return format.ToUpper()
                .Replace("A", version.Major.ToString())
                .Replace("B", version.Minor.ToString())
                .Replace("C", version.Build.ToString())
                .Replace("D", version.Revision.ToString());
        }
        return version.ToString();
    }
}
