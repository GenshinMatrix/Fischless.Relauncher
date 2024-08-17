namespace Fischless.Relauncher.Core.Relaunchs;

internal static class GenshinCopyCapture
{
    private static bool isEnabled = false;

    public static bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (isEnabled != value)
            {
                isEnabled = value;
                if (value)
                {
                    // TODO
                }
            }
        }
    }
}
