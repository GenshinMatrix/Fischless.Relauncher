using Relauncher.Core.Relaunchs;
using Relauncher.Relaunchs.Abstraction;

namespace Relauncher.Relaunchs;

public sealed class GenshinLauncherOption : IGameLauncherOption
{
    public string? GamePath { get; set; } = null;
    public string? WorkingDirectory { get; set; } = null;

    public GenshinAccount? Account { get; set; } = null;

    public GenshinArgumentsOption? Arguments { get; set; } = null;

    public GenshinAdvanceOption? Advance { get; set; } = null;

    public GenshinLinkageOption? Linkage { get; set; } = null;

    public GenshinGameRegion? Region { get; set; } = null;

    public IGameUnlockerOption Unlocker { get; set; } = GenshinUnlockerOption.Default.Value;
}
