using Relauncher.Relaunchs;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Relauncher.Core.Relaunchs;

internal sealed class GenshinFpsUnlocker(Process gameProcess)
{
    private readonly Process gameProcess = gameProcess;
    private int unlockFps;

    public GenshinFpsUnlocker SetTargetFps(int unlockFps)
    {
        this.unlockFps = unlockFps;
        return this;
    }

    public async Task UnlockAsync(GenshinUnlockerOption options, CancellationTokenSource cts = null!)
    {
        options.UnlockFps = unlockFps;
        await Task.Run(() => GameFpsUnlockerImpl.Start(options, pid: (uint)gameProcess.Id, cts: cts));
    }
}

internal sealed partial class GameFpsUnlockerImpl
{
    private static partial class Interop
    {
        [LibraryImport(@".\runtimes\win-x64\native\Fischless.UnlockerPatch.dll", EntryPoint = "unlock")]
        public static partial int Unlock(int pid, int targetFPS);
    }

    public static unsafe void Start(GenshinUnlockerOption option, string? gamePath = null, uint? pid = null, CancellationTokenSource? cts = null)
    {
        if (!option.UnlockFps.HasValue)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(gamePath) && pid == null)
        {
            return;
        }

        int targetPid = (int)pid!.Value;
        int targetFps = option.UnlockFps.Value;
        int ret = Interop.Unlock(targetPid, targetFps);

        Debug.WriteLine("[Unlocker] Unlock ret is " + ret);
    }
}
