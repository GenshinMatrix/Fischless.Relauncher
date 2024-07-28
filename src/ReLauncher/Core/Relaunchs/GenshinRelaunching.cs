using Relauncher.Relaunchs;
using System.Diagnostics;

namespace Relauncher.Core.Relaunchs;

public static class GenshinRelaunching
{
    public static async void StartAsync(GenshinLauncherOption option, CancellationToken? token = null)
    {
        while (!(token?.IsCancellationRequested ?? false))
        {
            Process? process = Process.GetProcessesByName("Yuanshen").FirstOrDefault();

            if (process != null)
            {
                int? parentProcessId = Interop.GetParentProcessId(process.Id);

                if (parentProcessId != null)
                {
                    string? parentWindowTitle = Interop.GetWindowTextByProcessId(parentProcessId.Value);

                    if (parentWindowTitle == "米哈游启动器")
                    {
                        process.Kill();
                        await GenshinLauncher.KillAsync(GenshinRelaunchMethod.Kill);
                        await GenshinLauncher.LaunchAsync(delayMs: 1000, relaunchMethod: GenshinRelaunchMethod.Kill);
                        continue;
                    }
                }
            }

            Thread.Sleep(1);
        }
    }
}
