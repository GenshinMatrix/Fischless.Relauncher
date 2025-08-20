namespace Fischless.Relauncher.Core.WindowsInput;

public static class WeakReferenceInputSimulator
{
    private static WeakReference<InputSimulator>? _weakRef;

    public static InputSimulator Instance
    {
        get
        {
            if (_weakRef == null || !_weakRef.TryGetTarget(out InputSimulator? sim))
            {
                sim = new InputSimulator();
                _weakRef = new WeakReference<InputSimulator>(sim);
            }
            return sim;
        }
    }

    public static IKeyboardSimulator Keyboard => Instance.Keyboard;

    public static IMouseSimulator Mouse => Instance.Mouse;

    public static IInputDeviceStateAdaptor InputDeviceState => Instance.InputDeviceState;
}
