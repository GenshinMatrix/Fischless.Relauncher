namespace Fischless.Relauncher.Relaunchs.Abstraction;

public interface IGameLauncherOption
{
    public string? GamePath { get; set; }

    public IGameUnlockerOption Unlocker { get; set; }
}
