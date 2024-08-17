using Fischless.Relauncher.Relaunchs;
using Fischless.Relauncher.Threading;
using Fischless.Relauncher.Win32;
using System.Diagnostics;
using System.IO;
using Vanara.PInvoke;
using Windows.System;

namespace Fischless.Relauncher.Core.Relaunchs;

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

        if (option.Account != null)
        {
            if (string.IsNullOrEmpty(option.Account.Server) || option.Account.Server == RegionCN)
            {
                if (!string.IsNullOrEmpty(option.Account?.Prod))
                {
                    GenshinRegistry.ProdCN = option.Account.Prod;
                }
            }
            else if (option.Account.Server == RegionOVERSEA)
            {
                if (!string.IsNullOrEmpty(option.Account.Prod))
                {
                    GenshinRegistry.ProdOVERSEA = option.Account.Prod;
                }
            }
        }

        if (option.Advance != null)
        {
            if (option.Advance.IsUseHDR)
            {
                if (Path.GetFileNameWithoutExtension(fileName).EndsWith("Genshin Impact"))
                {
                    GenshinRegistry.HDR_OVERSEA = 1;
                }
                else
                {
                    GenshinRegistry.HDR_CN = 1;
                }
            }
        }

        if (option.Linkage != null)
        {
            if (option.Linkage.IsUseReShade)
            {
                if (Directory.Exists(option.Linkage.ReShadePath))
                {
                    string? configPath = Path.Combine(option.Linkage.ReShadePath, "d3dx.ini");

                    if (File.Exists(configPath))
                    {
                        string[] profile = Interop.GetIniValues(configPath, "Loader", "target");

                        if (profile.Length != 0)
                        {
                            if (!(profile[0]?.Trim()?.Equals(fileName, StringComparison.OrdinalIgnoreCase) ?? false))
                            {
                                // Loader's ini format needs a space before the file name.
                                _ = Interop.SetIniValues(configPath, "Loader", "target", " " + fileName);
                            }
                        }
                    }

                    using FluentProcess loader = FluentProcess.Create()
                        .FileName(Path.Combine(option.Linkage.ReShadePath, "3DMigoto Loader.exe"))
                        .WorkingDirectory(option.Linkage.ReShadePath)
                        .Verb("runas");

                    if (option.Linkage.IsUseReShadeSilent)
                    {
                        _ = loader.CreateNoWindow()
                            .UseShellExecute(false)
                            .RedirectStandardOutput();
                    }

                    _ = loader.Start();

                    nint hWnd = await Task.Run(() =>
                    {
                        nint hWnd = IntPtr.Zero;

                        if (SpinWait.SpinUntil(() => loader.MainWindowHandle != IntPtr.Zero, 3000))
                        {
                            hWnd = loader.MainWindowHandle;
                        }
                        return hWnd;
                    });

                    if (option.Linkage.IsUseReShadeSilent)
                    {
                        if (hWnd != IntPtr.Zero)
                        {
                            _ = User32.ShowWindow(hWnd, ShowWindowCommand.SW_HIDE);
                        }
                    }

                    // Tactical avoidance
                    await Task.Delay(500);
                }
            }
        }

        if (option.Advance != null)
        {
            if (option.Advance.IsDisnetLaunching)
            {
                using FluentProcess process = FluentProcess.Create()
                    .FileName("netsh")
                    .Arguments($"advfirewall firewall add rule name=\"DIS_GENSHIN_NETWORK\" dir=out action=block program=\"{fileName}\"")
                    .CreateNoWindow()
                    .UseShellExecute(false)
                    .Verb("runas")
                    .Start()
                    .WaitForExit();

                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);

                    using FluentProcess process = FluentProcess.Create()
                        .FileName("netsh")
                        .Arguments("advfirewall firewall delete rule name=\"DIS_GENSHIN_NETWORK\"")
                        .CreateNoWindow()
                        .UseShellExecute(false)
                        .Verb("runas")
                        .Start()
                        .WaitForExit();
                });
            }
        }

        using Process? gameProcess = Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = true,
            FileName = fileName,
            Arguments = option.Arguments?.ToArguments(),
            WorkingDirectory = option.WorkingDirectory ?? new FileInfo(fileName).DirectoryName,
            Verb = "runas",
        });

        if (option.Linkage != null)
        {
            if (option.Linkage.IsUseBetterGI)
            {
                await Launcher.LaunchUriAsync(new Uri("bettergi://start"));
            }
        }

        if (option.Unlocker != null)
        {
            if (option.Unlocker.IsUnlockFps)
            {
                if (option.Unlocker.UnlockFps > 60)
                {
                    try
                    {
                        if (((GenshinUnlockerOption)option.Unlocker).UnlockFpsMethod == 0)
                        {
                            await new GenshinFpsUnlocker(gameProcess!)
                                .SetTargetFps((int)option.Unlocker.UnlockFps)
                                .UnlockAsync(GenshinUnlockerOption.Default.Value);
                        }
                        else
                        {
                            //await new GenshinFpsUnlocker1(gameProcess!)
                            //    .SetTargetFps((int)option.Unlocker.UnlockFps)
                            //    .UnlockAsync(GenshinUnlockerOption.Default.Value);
                        }
                    }
                    catch
                    {
                        ///
                    }
                }
            }
        }
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
                        await (callback?.Invoke(p) ?? Task.CompletedTask);
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

    public static nint TryGetHandleByWindowName()
    {
        HWND handle = User32.FindWindow("UnityWndClass", "原神");

        if (handle != 0)
        {
            return (nint)handle;
        }

        handle = User32.FindWindow("UnityWndClass", "Genshin Impact");
        if (handle != 0)
        {
            return (nint)handle;
        }

        handle = User32.FindWindow("Qt5152QWindowIcon", "云·原神");
        if (handle != 0)
        {
            return (nint)handle;
        }

        return IntPtr.Zero;
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
