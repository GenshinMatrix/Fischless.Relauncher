namespace Relauncher.Relaunchs.Abstraction;

public interface IUnlockerOption
{
    public int? TargetFps { get; set; }

    public int FindModuleDelay { get; set; }

    public int FindModuleLimit { get; set; }

    public int FpsDelay { get; set; }
}
