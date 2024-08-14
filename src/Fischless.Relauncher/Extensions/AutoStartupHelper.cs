﻿using Microsoft.Win32;
using System.Diagnostics;

namespace Fischless.Relauncher.Extensions;

internal static class AutoStartupHelper
{
    public static bool IsAutorun()
    {
        return RegistyAutoRunHelper.IsEnabled("miHoYo-reLauncher", $"\"{Process.GetCurrentProcess().MainModule?.FileName!}\" /autorun");
    }

    public static void RemoveAutorunShortcut()
    {
        RegistyAutoRunHelper.Disable("miHoYo-reLauncher");
    }

    public static void CreateAutorunShortcut()
    {
        RegistyAutoRunHelper.Enable("miHoYo-reLauncher", $"\"{Process.GetCurrentProcess().MainModule?.FileName!}\" /autorun");
    }
}

file static class RegistyAutoRunHelper
{
    private const string RunLocation = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public static void Enable(string keyName, string launchCommand)
    {
        using RegistryKey? key = Registry.CurrentUser.CreateSubKey(RunLocation);
        key?.SetValue(keyName, launchCommand);
    }

    public static bool IsEnabled(string keyName, string launchCommand)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunLocation);

        if (key == null)
        {
            return false;
        }

        string? value = (string?)key.GetValue(keyName);

        if (value == null)
        {
            return false;
        }

        return value == launchCommand;
    }

    public static void Disable(string keyName, string launchCommand = null!)
    {
        using RegistryKey? key = Registry.CurrentUser.CreateSubKey(RunLocation);

        _ = launchCommand;
        if (key == null)
        {
            return;
        }

        if (key.GetValue(keyName) != null)
        {
            key.DeleteValue(keyName);
        }
    }

    public static void SetEnabled(bool enable, string keyName, string launchCommand)
    {
        if (enable)
        {
            Enable(keyName, launchCommand);
        }
        else
        {
            Disable(keyName);
        }
    }
}
