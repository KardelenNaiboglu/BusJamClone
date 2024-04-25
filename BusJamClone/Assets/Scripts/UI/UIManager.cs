using TMPro;
using UnityEngine;
using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Transform winPanel;
    [SerializeField] private Transform failPanel;

    #region Zenject

    private SignalBus _signalBus;
    private SaveController _saveController;
    private GameController _gameController;

    [Inject]
    private void Construct(SignalBus signalBus,
        SaveController saveController,
        GameController gameController)
    {
        _signalBus = signalBus;
        _saveController = saveController;
        _gameController = gameController;
    }

    #endregion

    public void Init()
    {
        _signalBus.Subscribe<TimerChangedSignal>(OnTimerChangedSignal);
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
    }

    private void OnGameStateChangedSignal(GameStateChangedSignal signal)
    {
        switch (signal.GameState)
        {
            case GameState.Fail:
                failPanel.gameObject.SetActive(true);
                break;

            case GameState.Win:
                winPanel.gameObject.SetActive(true);
                break;

            default:
                SetLevel();
                winPanel.gameObject.SetActive(false);
                failPanel.gameObject.SetActive(false);
                break;
        }
    }

    private void OnTimerChangedSignal(TimerChangedSignal signal)
    {
        timerText.SetText(signal.TimeInSeconds.ToString());
    }

    private void SetLevel()
    {
        levelText.SetText($"Level {_saveController.GetCurrentLevelIndex() + 1}");
    }

    public void OnButtonClickPlay()
    {
        _gameController.ButtonClickPlay();
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<TimerChangedSignal>(OnTimerChangedSignal);
        _signalBus.TryUnsubscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
    }
}