using System.Text;

namespace Relauncher.Core.Relaunchs;

public sealed class GenshinArgumentsOption
{
    public bool IsUseArguments { get; set; } = false;

    public bool IsUseWindowModeExclusive { get; set; } = false;

    public bool IsScreenFullscreen { get; set; } = false;

    public bool IsPopupwindow { get; set; } = false;

    public bool IsPlatformTypeCloudThirdPartyMobile { get; set; } = false;

    public bool IsScreenWidth { get; set; } = false;

    public int ScreenWidth { get; set; } = 1920;

    public bool IsScreenHeight { get; set; } = false;

    public int ScreenHeight { get; set; } = 1080;

    public bool IsMonitor { get; set; } = false;

    public int Monitor { get; set; } = 1;

    public string ToArguments()
    {
        StringBuilder builder = new();

        if (IsUseArguments)
        {
            if (IsUseWindowModeExclusive)
            {
                builder.Append("-window-mode exclusive ");
            }

            if (IsScreenFullscreen)
            {
                builder.Append("-screen-fullscreen ");
            }

            if (IsPopupwindow)
            {
                builder.Append("-popupwindow ");
            }

            if (IsPlatformTypeCloudThirdPartyMobile)
            {
                builder.Append("-platform_type CLOUD_THIRD_PARTY_MOBILE ");
            }

            if (IsScreenWidth)
            {
                builder.Append($"-screen-width {ScreenWidth} ");
            }

            if (IsScreenHeight)
            {
                builder.Append($"-screen-height {ScreenHeight} ");
            }

            if (IsMonitor)
            {
                builder.Append($"-monitor {Monitor} ");
            }
        }

        return builder.ToString().TrimEnd();
    }

    public override string ToString() => ToArguments();
}
