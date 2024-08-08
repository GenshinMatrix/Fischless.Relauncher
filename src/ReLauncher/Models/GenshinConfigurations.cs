using System.ComponentModel;
using System.Reflection;

namespace Relauncher.Models;

[Obfuscation]
public class GenshinConfigurations
{
    public bool IsUseArguments { get; set; } = false;

    [Description("-window-mode exclusive")]
    public bool IsUseWindowModeExclusive { get; set; } = false;

    [Description("-screen-fullscreen")]
    public bool IsScreenFullscreen { get; set; } = false;

    [Description("-popupwindow")]
    public bool IsPopupwindow { get; set; } = false;

    [Description("-platform_type CLOUD_THIRD_PARTY_MOBILE")]
    public bool IsPlatformTypeCloudThirdPartyMobile { get; set; } = false;

    [Description("-screen-width")]
    public bool IsScreenWidth { get; set; } = false;

    [Description("-screen-width")]
    public int ScreenWidth { get; set; } = 1920;

    [Description("-screen-height")]
    public bool IsScreenHeight { get; set; } = false;

    [Description("-screen-height")]
    public int ScreenHeight { get; set; } = 1080;

    [Description("-monitor")]
    public bool IsMonitor { get; set; } = false;

    [Description("-monitor")]
    public int Monitor { get; set; } = 1;
}
