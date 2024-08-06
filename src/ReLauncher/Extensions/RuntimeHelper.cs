using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using Application = System.Windows.Application;

namespace Relauncher.Extensions;

internal static class RuntimeHelper
{
    public static bool IsElevated { get; } = GetElevated();

    private static bool GetElevated()
    {
        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    public static void CheckSingleInstance(string instanceName, Action<bool> callback = null!)
    {
        EventWaitHandle? handle;

        try
        {
            handle = EventWaitHandle.OpenExisting(instanceName);
            handle.Set();
            callback?.Invoke(false);
            Environment.Exit(0xFFFF);
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            callback?.Invoke(true);
            handle = new EventWaitHandle(false, EventResetMode.AutoReset, instanceName);
        }
        GC.KeepAlive(handle);
        GC.KeepAlive(Task.Factory.StartNew(() =>
        {
            while (handle.WaitOne())
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Application.Current.MainWindow?.Activate();
                    Application.Current.MainWindow?.Show();
                });
            }
        }, TaskCreationOptions.LongRunning));
    }

    public static void Restart(string fileName = null!, string dir = null!, string args = null!, int? exitCode = null, bool forced = false)
    {
        _ = args;

        try
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = fileName ?? Path.Combine(dir ?? AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName),
                    WorkingDirectory = dir ?? Environment.CurrentDirectory,
                    UseShellExecute = true,
                },
            };
            process.Start();
        }
        catch (Win32Exception)
        {
            return;
        }
        if (forced)
        {
            Process.GetCurrentProcess().Kill();
        }
        Environment.Exit(exitCode ?? 'r' + 'e' + 's' + 't' + 'a' + 'r' + 't');
    }
}
