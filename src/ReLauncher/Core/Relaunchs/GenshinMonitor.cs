using Gma.System.MouseKeyHook;
using Relauncher.Models;

namespace Relauncher.Core.Relaunchs;

internal sealed class GenshinMonitor
{
    public static Lazy<GenshinMonitor> Instance { get; } = new();

    private IKeyboardMouseEvents? _globalHook;

    public GenshinMonitor()
    {
        _globalHook = Hook.GlobalEvents();

        _globalHook.KeyDown += GlobalHookKeyDown;
        _globalHook.KeyUp += GlobalHookKeyUp;
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
                GenshinClicker.IsUseAutoClick = !GenshinClicker.IsUseAutoClick;
            }
        }
    }
}
