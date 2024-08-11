using Relauncher.Relaunchs.Abstraction;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Relauncher.Relaunchs;

public sealed class GenshinUnlockerOption : IGameUnlockerOption
{
    public static Lazy<GenshinUnlockerOption> Default { get; } = new();

    public bool IsUnlockFps { get; set; } = false;

    [Description("UnlockFps")]
    public int? TargetFps { get; set; } = 120;

    public int UnlockFpsMethod { get; set; } = 0;

    [Obsolete]
    [SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message")]
    public int FindModuleDelay { get; set; } = 100;

    [Obsolete]
    [SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message")]
    public int FindModuleLimit { get; set; } = 2000;

    [Obsolete]
    [SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message")]
    public int FpsDelay { get; set; } = 2000;
}
