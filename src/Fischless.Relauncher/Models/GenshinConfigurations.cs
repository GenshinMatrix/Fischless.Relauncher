using System.ComponentModel;
using System.Reflection;

namespace Fischless.Relauncher.Models;

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

    public bool IsUnlockFps { get; set; } = false;

    public int UnlockFps { get; set; } = 60;

    public int UnlockFpsMethod { get; set; } = 0;

    public bool IsShowFps { get; set; } = false;

    public bool IsDisnetLaunching { get; set; } = false;

    public bool IsDarkMode { get; set; } = false;

    public bool IsSquareCorner { get; set; } = false;

    public bool IsUseHDR { get; set; } = false;

    public string? ReShadePath { get; set; } = null;

    public bool IsUseReShade { get; set; } = false;

    public bool IsUseReShadeSilent { get; set; } = false;

    public bool IsUseBetterGI { get; set; } = false;

    public bool IsUseBorderless { get; set; } = false;

    public bool IsUseQuickScreenshot { get; set; } = false;

    public bool IsUseQuickScreenshotHideUID { get; set; } = false;

    public bool IsUseAutoMute { get; set; } = false;

    public bool IsUseAutoClick { get; set; } = false;
}
