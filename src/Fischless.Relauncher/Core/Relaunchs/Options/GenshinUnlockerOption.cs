using Fischless.Relauncher.Relaunchs.Abstraction;

namespace Fischless.Relauncher.Relaunchs;

public sealed class GenshinUnlockerOption : IGameUnlockerOption
{
    public static Lazy<GenshinUnlockerOption> Default { get; } = new();

    public bool IsUnlockFps { get; set; } = false;

    public int? UnlockFps { get; set; } = 120;

    public int UnlockFpsMethod { get; set; } = 0;
}
