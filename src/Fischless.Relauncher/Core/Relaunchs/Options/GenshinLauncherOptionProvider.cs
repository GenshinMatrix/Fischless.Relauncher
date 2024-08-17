using Fischless.Relauncher.Models;
using Fischless.Relauncher.Relaunchs;

namespace Fischless.Relauncher.Core.Relaunchs;

internal static class GenshinLauncherOptionProvider
{
    public static GenshinLauncherOption GetOption()
    {
        GenshinConfigurations config = Configurations.Genshin.Get();
        GenshinLauncherOption option = new()
        {
            GamePath = null,
            WorkingDirectory = null,
            Account = null!, // TODO: Not supported
            Arguments = new GenshinArgumentsOption()
            {
                IsUseArguments = config.IsUseArguments,
                IsUseWindowModeExclusive = config.IsUseWindowModeExclusive,
                IsScreenFullscreen = config.IsScreenFullscreen,
                IsPopupwindow = config.IsPopupwindow,
                IsPlatformTypeCloudThirdPartyMobile = config.IsPlatformTypeCloudThirdPartyMobile,
                IsScreenWidth = config.IsScreenWidth,
                ScreenWidth = config.ScreenWidth,
                IsScreenHeight = config.IsScreenHeight,
                ScreenHeight = config.ScreenHeight,
                IsMonitor = config.IsMonitor,
                Monitor = config.Monitor,
            },
            Advance = new GenshinAdvanceOption()
            {
                IsDisnetLaunching = config.IsDisnetLaunching,
                IsUseHDR = config.IsUseHDR,
            },
            Linkage = new GenshinLinkageOption()
            {
                ReShadePath = config.ReShadePath,
                IsUseReShade = config.IsUseReShade,
                IsUseReShadeSilent = config.IsUseReShadeSilent,
                IsUseBetterGI = config.IsUseBetterGI,
            },
            Region = null,
            Unlocker = new GenshinUnlockerOption()
            {
                IsUnlockFps = config.IsUnlockFps,
                UnlockFps = config.UnlockFps,
                UnlockFpsMethod = config.UnlockFpsMethod,
            },
        };

        return option;
    }
}
