using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace Fischless.Relauncher.Core.WindowsInput;

internal class WindowsInputMessageDispatcher : IInputMessageDispatcher
{
    public void DispatchInput(User32.INPUT[] inputs)
    {
        ArgumentNullException.ThrowIfNull(inputs);

        if (inputs.Length == 0)
        {
            throw new ArgumentException("The input array was empty", nameof(inputs));
        }

        uint num = User32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<User32.INPUT>());

        if (num != (ulong)(long)inputs.Length)
        {
            _ = new Exception(
                """
                Simulation key and mouse message sending failed! Common reasons:
                    1. You do not run the program with administrator privileges;
                    2. There is security software interception;
                """);
        }
    }
}
