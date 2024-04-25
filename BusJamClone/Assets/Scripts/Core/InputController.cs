using System;
using Lean.Touch;
using Zenject;

public class InputController : IInitializable, IDisposable
{
    #region Zenject

    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    #endregion

    public void Initialize()
    {
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUpdate += OnFingerUpdate;
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    private void OnFingerDown(LeanFinger obj)
    {
        _signalBus.Fire(new FingerDownSignal(obj.ScreenPosition));
    }

    private void OnFingerUpdate(LeanFinger obj)
    {
        _signalBus.Fire(new FingerUpdateSignal(obj.ScreenPosition));
    }

    private void OnFingerUp(LeanFinger obj)
    {
        _signalBus.Fire(new FingerUpSignal(obj.ScreenPosition));
    }

    public void Dispose()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUpdate -= OnFingerUpdate;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }
}