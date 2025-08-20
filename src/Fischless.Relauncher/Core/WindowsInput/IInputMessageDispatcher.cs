using Vanara.PInvoke;

namespace Fischless.Relauncher.Core.WindowsInput;

internal interface IInputMessageDispatcher
{
    public void DispatchInput(User32.INPUT[] inputs);
}
