namespace Fischless.Relauncher.Threading;

public class DeferManager(List<Action> deferredActions) : IDisposable
{
    private readonly List<Action> deferredActions = deferredActions;

    public DeferManager() : this([])
    {
    }

    public void Defer(Action action)
    {
        deferredActions.Add(action);
    }

    public void Dispose()
    {
        deferredActions.ForEach(action => action?.Invoke());
        deferredActions.Clear();
    }
}
