using System;
using Cysharp.Threading.Tasks;
using Zenject;

public class TimerController : IInitializable, IDisposable
{
    #region Zenject

    private SignalBus _signalBus;

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    #endregion

    public int LeftTime { get; private set; }
    private bool _isTimerRunning = false;

    public void Initialize()
    {
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
    }

    private void OnGameStateChangedSignal(GameStateChangedSignal signal)
    {
        switch (signal.GameState)
        {
            case GameState.Playing:
                _isTimerRunning = true;
                StartTimer();
                break;

            default:
                _isTimerRunning = false;
                break;
        }
    }

    public void SetTimer(int timeInSeconds)
    {
        LeftTime = timeInSeconds;
        _signalBus.Fire(new TimerChangedSignal(LeftTime));
    }

    private void StartTimer()
    {
        Timer().Forget();
    }

    private async UniTaskVoid Timer()
    {
        while (_isTimerRunning)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            if (!_isTimerRunning) break;

            LeftTime--;

            _signalBus.Fire(new TimerChangedSignal(LeftTime));

            if (LeftTime <= 0)
            {
                _signalBus.Fire<TimesUpSignal>();
                break;
            }
        }
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
    }
}