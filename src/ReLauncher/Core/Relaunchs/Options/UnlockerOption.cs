using Relauncher.Relaunchs.Abstraction;

namespace Relauncher.Relaunchs;

public sealed class UnlockerOption : IUnlockerOption
{
    public static Lazy<UnlockerOption> Default { get; } = new();

    public int? TargetFps { get; set; } = 120;

    public int FindModuleDelay { get; set; } = 100;

    public int FindModuleLimit { get; set; } = 2000;

    public int FpsDelay { get; set; } = 2000;
}
