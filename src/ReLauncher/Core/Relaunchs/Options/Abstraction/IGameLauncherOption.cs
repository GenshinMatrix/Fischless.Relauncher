namespace Relauncher.Relaunchs.Abstraction;

public interface IGameLauncherOption
{
    public string? GamePath { get; set; }

    public IUnlockerOption Unlocker { get; set; }
}
