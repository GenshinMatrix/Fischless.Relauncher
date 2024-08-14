using Microsoft.Win32;
using Fischless.Relauncher.Core.Loggers;
using Fischless.Relauncher.Extensions;
using Fischless.Relauncher.Relaunchs;
using Fischless.Relauncher.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Fischless.Relauncher.Core.Relaunchs;

public static partial class GenshinRegistry
{
    public static string ProdCN
    {
        get => GetStringFromRegedit(GenshinRegistryKeys.PROD_CN, GenshinGameRegion.CN);
        set => SetStringToRegedit(GenshinRegistryKeys.PROD_CN, value, GenshinGameRegion.CN);
    }

    public static string ProdOVERSEA
    {
        get => GetStringFromRegedit(GenshinRegistryKeys.PROD_OVERSEA, GenshinGameRegion.OVERSEA);
        set => SetStringToRegedit(GenshinRegistryKeys.PROD_OVERSEA, value, GenshinGameRegion.OVERSEA);
    }

    [Obsolete("Unusable")]
    public static string DataCN
    {
        get => GetStringFromRegedit(GenshinRegistryKeys.DATA, GenshinGameRegion.CN);
        set => SetStringToRegedit(GenshinRegistryKeys.DATA, value, GenshinGameRegion.CN);
    }

    [Obsolete("Unusable")]
    public static string DataOVERSEA
    {
        get => GetStringFromRegedit(GenshinRegistryKeys.DATA, GenshinGameRegion.OVERSEA);
        set => SetStringToRegedit(GenshinRegistryKeys.DATA, value, GenshinGameRegion.OVERSEA);
    }

    public static int HDR_CN
    {
        get => GetWindowsHDR(GenshinGameRegion.CN);
        set => SetWindowsHDR(GenshinGameRegion.CN, value);
    }

    public static int HDR_OVERSEA
    {
        get => GetWindowsHDR(GenshinGameRegion.OVERSEA);
        set => SetWindowsHDR(GenshinGameRegion.OVERSEA, value);
    }

    public static void SetWindowsHDR(GenshinGameRegion type, int value)
    {
        Registry.SetValue(type.GetRegKeyName(), GenshinRegistryKeys.HDR, value);
    }

    public static int GetWindowsHDR(GenshinGameRegion type)
    {
        object? value = Registry.GetValue(type.GetRegKeyName(), GenshinRegistryKeys.HDR, 0);
        return (int)value!;
    }

    public static string GetStringFromRegedit(string key, GenshinGameRegion type = GenshinGameRegion.CN)
    {
        if (RuntimeHelper.IsElevated)
        {
            return GetStringFromRegeditDirect(key, type);
        }

        try
        {
            using MemoryStream stream = new();
            FluentProcess.Create()
                .FileName("powershell")
                .WorkingDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                .Arguments(@$"Get-ItemPropertyValue -Path 'HKCU:\Software\miHoYo\{type.ParseGameType()}' -Name '{type.GetRegKey()}';")
                .UseShellExecute(false)
                .CreateNoWindow()
                .RedirectStandardOutput()
                .Start()
                .BeginOutputRead(stream)
                .WaitForExit();
            stream.Position = 0;
            string lines = Encoding.UTF8.GetString(stream.ToArray());
            StringBuilder sb = new();

            foreach (string line in lines.Replace("\r", string.Empty).Split('\n'))
            {
                if (byte.TryParse(line, out byte b))
                {
                    sb.Append((char)b);
                }
            }
            Log.Debug(sb.ToString());
            return sb.ToString();
        }
        catch (Exception e)
        {
            Log.Warning(e.ToString());
        }
        return null!;
    }

    public static string GetStringFromRegeditDirect(string key, GenshinGameRegion type = GenshinGameRegion.CN)
    {
        object? value = Registry.GetValue(type.GetRegKeyName(), key, string.Empty);

        if (value is byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        return null!;
    }

    public static void SetStringToRegedit(string key, string value, GenshinGameRegion type = GenshinGameRegion.CN)
    {
        if (RuntimeHelper.IsElevated)
        {
            SetStringToRegeditDirect(key, value, type);
            return;
        }

        try
        {
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            string script = $"""
            $value = [Convert]::FromBase64String('{base64}');
            Set-ItemProperty -Path 'HKCU:\Software\miHoYo\{type.ParseGameType()}' -Name '{type.GetRegKey()}' -Value $value -Force;
            """;
            Process.Start(new ProcessStartInfo()
            {
                FileName = "powershell",
                Arguments = script,
                CreateNoWindow = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            })?.WaitForExit();
        }
        catch (Exception e)
        {
            Log.Warning(e.ToString());
        }
    }

    public static void SetStringToRegeditDirect(string key, string value, GenshinGameRegion type = GenshinGameRegion.CN)
    {
        Registry.SetValue(GetRegKeyName(type), key, Encoding.UTF8.GetBytes(value));
    }

    public static string GetRegKey(this GenshinGameRegion type)
    {
        return type switch
        {
            GenshinGameRegion.OVERSEA => GenshinRegistryKeys.PROD_OVERSEA,
            GenshinGameRegion.CN_CLOUD => GenshinRegistryKeys.PROD_CNCloud,
            GenshinGameRegion.CN or _ => GenshinRegistryKeys.PROD_CN,
        };
    }

    public static string GetRegKeyName(this GenshinGameRegion type)
    {
        return @$"{GenshinRegistryKeys.HKEY_CURRENT_USER}\SOFTWARE\miHoYo\" + ParseGameType(type);
    }

    public static string ParseGameType(this GenshinGameRegion type)
    {
        return type switch
        {
            GenshinGameRegion.OVERSEA => GenshinRegistryKeys.OVERSEA,
            GenshinGameRegion.CN_CLOUD => GenshinRegistryKeys.CNCloud,
            GenshinGameRegion.CN or _ => GenshinRegistryKeys.CN,
        };
    }
}

public sealed class GenshinRegistryKeys
{
    public const string HKEY_CLASSES_ROOT = "HKEY_CLASSES_ROOT";
    public const string HKEY_CURRENT_USER = "HKEY_CURRENT_USER";
    public const string HKEY_LOCAL_MACHINE = "HKEY_LOCAL_MACHINE";
    public const string HKEY_USERS = "HKEY_USERS";
    public const string HKEY_PERFORMANCE_DATA = "HKEY_PERFORMANCE_DATA";
    public const string HKEY_CURRENT_CONFIG = "HKEY_CURRENT_CONFIG";

    public const string CN = "原神";
    public const string PROD_CN = "MIHOYOSDK_ADL_PROD_CN_h3123967166";
    public const string DATA = "GENERAL_DATA_h2389025596";

    public const string OVERSEA = "Genshin Impact";
    public const string PROD_OVERSEA = "MIHOYOSDK_ADL_PROD_OVERSEA_h1158948810";

    public const string HDR = "WINDOWS_HDR_ON_h3132281285";

    public const string CNCloud = "云·原神";
    public const string PROD_CNCloud = "MIHOYOSDK_ADL_0";
}
