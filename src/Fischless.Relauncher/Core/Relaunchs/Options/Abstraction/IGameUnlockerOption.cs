namespace Fischless.Relauncher.Relaunchs.Abstraction;

public interface IGameUnlockerOption
{
    public bool IsUnlockFps { get; set; }

    public int? UnlockFps { get; set; }
}
