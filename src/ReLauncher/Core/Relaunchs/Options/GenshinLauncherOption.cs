using Relauncher.Core.Relaunchs;
using Relauncher.Relaunchs.Abstraction;

namespace Relauncher.Relaunchs;

public sealed class GenshinLauncherOption : IGameLauncherOption
{
    public string? GamePath { get; set; } = null;
    public string? WorkingDirectory { get; set; } = null;

    public GenshinAccount? Account { get; set; } = null;

    public GenshinGameRegion? Region { get; set; } = null;

    public IUnlockerOption Unlocker { get; set; } = UnlockerOption.Default.Value;
}
