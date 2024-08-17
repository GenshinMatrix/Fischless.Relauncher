using Fischless.Relauncher.Models;
using Gma.System.MouseKeyHook;

namespace Fischless.Relauncher.Core.Relaunchs;

internal sealed class GenshinMonitor
{
    public static Lazy<GenshinMonitor> Instance { get; } = new();

    public IKeyboardMouseEvents GlobalHook { get; }

    public GenshinMonitor()
    {
        GlobalHook = Hook.GlobalEvents();
        GlobalHook.KeyDown += GlobalHookKeyDown;
        GlobalHook.KeyUp += GlobalHookKeyUp;
    }

    private void GlobalHookKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.D6)
        {
            ///
        }
    }

    private void GlobalHookKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.D6)
        {
            if (Configurations.Genshin.Get().IsUseAutoClick)
            {
                GenshinClicker.IsEnabled = !GenshinClicker.IsEnabled;
            }
        }
    }
}
