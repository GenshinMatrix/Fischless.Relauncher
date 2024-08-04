using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Vanara.PInvoke;

namespace Relauncher;

public static class Interop
{
    public static unsafe int? GetParentProcessId(int pid)
    {
        using var hProcess = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_READ, false, (uint)pid);

        if (hProcess == IntPtr.Zero)
        {
            return null!;
        }

        NtDll.PROCESS_BASIC_INFORMATION pbi = new();
        NTStatus status = NtDll.NtQueryInformationProcess(hProcess, NtDll.PROCESSINFOCLASS.ProcessBasicInformation, (nint)(&pbi), (uint)Marshal.SizeOf<NtDll.PROCESS_BASIC_INFORMATION>(), out var returnLength);

        if (status == NTStatus.STATUS_SUCCESS)
        {
            return (int)pbi.InheritedFromUniqueProcessId;
        }
        else
        {
            return null!;
        }
    }

    public static string? GetWindowTextByProcessId(int pid)
    {
        string? windowTitle = null;

        User32.EnumWindows((hWnd, lParam) =>
        {
            _ = User32.GetWindowThreadProcessId(hWnd, out uint processId);

            if (processId == pid)
            {
                StringBuilder title = new(256);
                _ = User32.GetWindowText(hWnd, title, title.Capacity);
                windowTitle = title.ToString();
                return false;
            }
            return true;
        }, IntPtr.Zero);

        return windowTitle;
    }

    public static uint GetProcessIdByName(string processName)
    {
        uint pid = 0;
        Kernel32.PROCESSENTRY32 pe32 = new()
        {
            dwSize = (uint)Marshal.SizeOf(typeof(Kernel32.PROCESSENTRY32))
        };

        using Kernel32.SafeHSNAPSHOT snap = Kernel32.CreateToolhelp32Snapshot(Kernel32.TH32CS.TH32CS_SNAPPROCESS, 0);
        if (!snap.IsInvalid)
        {
            if (Kernel32.Process32First(snap, ref pe32))
            {
                do
                {
                    if (pe32.szExeFile.Equals(processName, StringComparison.OrdinalIgnoreCase))
                    {
                        pid = pe32.th32ProcessID;
                        break;
                    }
                } while (Kernel32.Process32Next(snap, ref pe32));
            }
        }
        return pid;
    }

    public static string? GetExeNameByProcessId(uint pid)
    {
        using Kernel32.SafeHPROCESS hProcess = Kernel32.OpenProcess(new ACCESS_MASK(Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION), false, pid);

        if (!hProcess.IsInvalid)
        {
            StringBuilder exeName = new(260);
            uint size = (uint)exeName.Capacity;

            if (Kernel32.QueryFullProcessImageName(hProcess, Kernel32.PROCESS_NAME.PROCESS_NAME_WIN32, exeName, ref size))
            {
                return exeName.ToString();
            }
            else
            {
                Debug.WriteLine($"Error getting process image file name. Error code: {Marshal.GetLastWin32Error()}");
            }
        }
        return null;
    }

    public static string GetLastErrorAsString(Win32Error errorCode)
    {
        StringBuilder messageBuffer = new(256);
        int formatResult = Kernel32.FormatMessage(
            Kernel32.FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM | Kernel32.FormatMessageFlags.FORMAT_MESSAGE_IGNORE_INSERTS,
            IntPtr.Zero,
            (uint)errorCode,
            0,
            messageBuffer,
            (uint)messageBuffer.Capacity,
            IntPtr.Zero
        );

        if (formatResult == NTStatus.STATUS_SUCCESS)
        {
            return $"Unknown error (Code {errorCode})";
        }

        return messageBuffer.ToString().Trim();
    }

    public static HRESULT DwmSetWindowAttribute(HWND hWnd, DwmApi.DWMWINDOWATTRIBUTE dwAttribute, int pvAttribute, int cbAttribute)
    {
        nint pvAttributePtr = Marshal.AllocHGlobal(sizeof(int));
        Marshal.WriteInt32(pvAttributePtr, pvAttribute);

        try
        {
            return DwmApi.DwmSetWindowAttribute(
                hWnd,
                dwAttribute,
                pvAttributePtr,
                cbAttribute
            );
        }
        finally
        {
            Marshal.FreeHGlobal(pvAttributePtr);
        }
    }
}
