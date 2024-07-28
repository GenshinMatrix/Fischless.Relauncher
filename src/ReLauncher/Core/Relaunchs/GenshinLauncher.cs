using Relauncher.Relaunchs;
using System.Diagnostics;
using System.IO;

namespace Relauncher.Core.Relaunchs;

internal class GenshinLauncher
{
    public const string RegionCN = "CN";
    public const string RegionOVERSEA = "OVERSEA";

    public const string ProcessNameCN = "YuanShen";
    public const string ProcessNameOVERSEA = "GenshinImpact";
    public const string ProcessNameCloud = "Genshin Impact Cloud Game";

    public const string FileNameCN = "YuanShen.exe";
    public const string FileNameOVERSEA = "GenshinImpact.exe";
    public const string FileNameCloud = "Genshin Impact Cloud Game.exe";

    public const string FolderName = "Genshin Impact Game";

    public static bool TryGetProcess(out Process? process)
    {
        try
        {
            string[] names = [ProcessNameCN, ProcessNameOVERSEA];

            foreach (string name in names)
            {
                Process[] ps = Process.GetProcessesByName(name);

                foreach (Process? p in ps)
                {
                    process = p;
                    return true;
                }
            }
        }
        catch
        {
        }
        process = null!;
        return false;
    }

    public static bool TryGetProcessRegion(out string region)
    {
        region = null!;
        try
        {
            string[] names = [ProcessNameCN, ProcessNameOVERSEA];

            foreach (string name in names)
            {
                Process[] ps = Process.GetProcessesByName(name);

                foreach (Process? p in ps)
                {
                    region = name switch
                    {
                        ProcessNameOVERSEA => RegionOVERSEA,
                        ProcessNameCN or _ => RegionCN,
                    };
                    return true;
                }
            }
        }
        catch
        {
        }
        return false;
    }

    public static bool TryClose()
    {
        return TryGetProcess(out Process? p) && (p?.CloseMainWindow() ?? false);
    }

    public static bool TryKill()
    {
        bool got = TryGetProcess(out Process? p);
        p?.Kill();
        return got && p != null;
    }

    public static bool TryGetGamePath(out string gamePath)
    {
        string fileName = Path.Combine(GenshinRegistry.InstallPathCN ?? string.Empty, FolderName, FileNameCN);

        if (!File.Exists(fileName))
        {
            fileName = Path.Combine(GenshinRegistry.InstallPathOVERSEA ?? string.Empty, FolderName, FileNameOVERSEA);
        }
        return !string.IsNullOrEmpty(gamePath = fileName);
    }

    public static bool TryGetGamePath(string launchParameterGamePath, out string gamePath)
    {
        string fileName = null!;

        if (string.IsNullOrWhiteSpace(launchParameterGamePath))
        {
            _ = TryGetGamePath(out fileName);
        }
        else
        {
            FileInfo fileInfo = new(launchParameterGamePath);

            if (fileInfo.Name.Equals(FileNameCN, StringComparison.OrdinalIgnoreCase)
             || fileInfo.Name.Equals(FileNameOVERSEA, StringComparison.OrdinalIgnoreCase))
            {
                fileName = launchParameterGamePath;
            }

            if (!File.Exists(fileName))
            {
                if (string.IsNullOrEmpty(GenshinRegistry.InstallPathCN) && string.IsNullOrEmpty(GenshinRegistry.InstallPathOVERSEA))
                {
                    throw new Exception("Genshin Impact not installed.");
                }
                _ = TryGetGamePath(out fileName);
            }
        }

        return !string.IsNullOrEmpty(gamePath = fileName);
    }

    public static async Task KillAsync(GenshinRelaunchMethod relaunchMethod = GenshinRelaunchMethod.None)
    {
        try
        {
            if (relaunchMethod switch
            {
                GenshinRelaunchMethod.Kill => await TryKillAsync(),
                GenshinRelaunchMethod.Close => await TryCloseAsync(),
                _ => false,
            })
            {
                if (!SpinWait.SpinUntil(() => TryGetProcess(out _), 15000))
                {
                    throw new Exception("Failed to kill Genshin Impact.");
                }
            }
        }
        catch
        {
            throw;
        }
    }

    public static async Task LaunchAsync(int? delayMs = null, GenshinRelaunchMethod relaunchMethod = GenshinRelaunchMethod.None, GenshinLauncherOption option = null!)
    {
        await KillAsync(relaunchMethod);

        if (delayMs != null)
        {
            await Task.Delay((int)delayMs);
        }

        option ??= new();

        _ = TryGetGamePath(option.GamePath!, out string fileName);

        if (string.IsNullOrEmpty(option?.Account?.Server) || option.Account.Server == RegionCN)
        {
            if (!string.IsNullOrEmpty(option?.Account?.Prod))
            {
                GenshinRegistry.ProdCN = option.Account.Prod;
            }
        }
        else if (option?.Account?.Server == RegionOVERSEA)
        {
            if (!string.IsNullOrEmpty(option?.Account?.Prod))
            {
                GenshinRegistry.ProdOVERSEA = option.Account.Prod;
            }
        }

        Process? gameProcess = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = true,
            FileName = fileName,
            Arguments = option!.ToString(), // TODO
            WorkingDirectory = option.WorkingDirectory ?? new FileInfo(fileName).DirectoryName,
            Verb = "runas",
        });

#if false // Not stabled to unlock.
        if (launchParameter.Fps > 60)
        {
            try
            {
                await new GameFpsUnlocker(gameProcess)
                    .SetTargetFps((int)launchParameter.Fps.Value)
                    .UnlockAsync(GameFpsUnlockerOption.Default.Value);
            }
            catch
            {
            }
        }
#endif
    }

    public static async Task<bool> TryGetProcessAsync(Func<Process?, Task> callback = null!)
    {
        return await Task.Run(async () =>
        {
            try
            {
                string[] names = [ProcessNameCN, ProcessNameOVERSEA];

                foreach (string name in names)
                {
                    Process[] ps = Process.GetProcessesByName(name);

                    foreach (Process? p in ps)
                    {
                        await callback?.Invoke(p)!;
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        });
    }

    public static async Task<bool> TryCloseAsync()
    {
        return await TryGetProcessAsync(p =>
        {
            p?.CloseMainWindow();
            return Task.CompletedTask;
        });
    }

    public static async Task<bool> TryKillAsync()
    {
        return await TryGetProcessAsync(p =>
        {
            p?.Kill();
            return Task.CompletedTask;
        });
    }
}

public enum GenshinRelaunchMethod
{
    None,
    Kill,
    Close,
}
