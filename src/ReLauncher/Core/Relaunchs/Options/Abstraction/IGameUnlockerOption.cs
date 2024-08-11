namespace Relauncher.Relaunchs.Abstraction;

public interface IGameUnlockerOption
{
    public int? TargetFps { get; set; }

    [Obsolete]
    public int FindModuleDelay { get; set; }

    [Obsolete]
    public int FindModuleLimit { get; set; }

    [Obsolete]
    public int FpsDelay { get; set; }
}
