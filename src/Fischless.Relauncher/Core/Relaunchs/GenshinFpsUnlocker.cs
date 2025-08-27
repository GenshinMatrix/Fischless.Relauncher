using Fischless.Relauncher.Relaunchs;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnlockerPatch;

namespace Fischless.Relauncher.Core.Relaunchs;

internal sealed class GenshinFpsUnlocker(string gamePath, string? cli = null)
{
    private readonly string gamePath = gamePath;
    private readonly string? cli = cli;
    private int unlockFps;

    public GenshinFpsUnlocker SetTargetFps(int unlockFps)
    {
        this.unlockFps = unlockFps;
        return this;
    }

    public async Task UnlockAsync(GenshinUnlockerOption options, CancellationTokenSource cts = null!)
    {
        options.UnlockFps = unlockFps;
        await Task.Run(() => GameFpsUnlockerImpl.Start(options, gamePath, cli, cts: cts));
    }
}

internal sealed partial class GameFpsUnlockerImpl
{
    public static void Start(GenshinUnlockerOption option, string? gamePath = null, string? cli = null, CancellationTokenSource? cts = null)
    {
        if (!option.UnlockFps.HasValue)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(gamePath))
        {
            return;
        }

        int targetFps = option.UnlockFps.Value;
        bool ret = UnlockerLauncher.Start(gamePath, targetFps, cli);

        Debug.WriteLine("[Unlocker] Unlock ret is " + ret);
    }
}
