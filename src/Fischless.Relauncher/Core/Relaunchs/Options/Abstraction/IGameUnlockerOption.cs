using System.Diagnostics.CodeAnalysis;

namespace Fischless.Relauncher.Relaunchs.Abstraction;

public interface IGameUnlockerOption
{
    public bool IsUnlockFps { get; set; }

    public int? UnlockFps { get; set; }

    [Obsolete]
    [SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message")]
    public int FindModuleDelay { get; set; }

    [Obsolete]
    [SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message")]
    public int FindModuleLimit { get; set; }

    [Obsolete]
    [SuppressMessage("Design", "CA1041:Provide ObsoleteAttribute message")]
    public int FpsDelay { get; set; }
}
