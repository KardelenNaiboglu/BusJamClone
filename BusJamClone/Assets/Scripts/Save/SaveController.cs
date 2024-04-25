using System;
using System.Collections.Generic;
using Zenject;

public class SaveController : IInitializable, IDisposable
{
    #region Zenject

    private BoardCoordinateSystem _boardCoordinateSystem;
    private GameController _gameController;
    private QueueController _queueController;
    private BusController _busController;
    private TimerController _timerController;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(BoardCoordinateSystem boardCoordinateSystem,
        GameController gameController,
        QueueController queueController,
        BusController busController,
        TimerController timerController,
        SignalBus signalBus)
    {
        _boardCoordinateSystem = boardCoordinateSystem;
        _gameController = gameController;
        _queueController = queueController;
        _busController = busController;
        _timerController = timerController;
        _signalBus = signalBus;
    }

    #endregion

    private readonly string _levelSave = "Level";
    private readonly string _lastGameSave = "LastGameSave";

    public void Initialize()
    {
        _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
    }

    private void OnGameStateChangedSignal(GameStateChangedSignal signal)
    {
        switch (signal.GameState)
        {
            case GameState.Win:
                OnLevelWin();
                break;

            default:
                break;
        }
    }

    private void OnLevelWin()
    {
        var settings = new ES3Settings(ES3.Location.Cache);

        ES3.Save(_levelSave, GetCurrentLevelIndex() + 1, settings);
        ES3.StoreCachedFile();
    }

    public int GetCurrentLevelIndex()
    {
        return ES3.Load(_levelSave, 0);
    }

    public void SaveCurrentGame()
    {
        if (_gameController.GameState != GameState.Playing)
        {
            return;
        }

        var settings = new ES3Settings(ES3.Location.Cache);

        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.LeftTime = _timerController.LeftTime;
        gameSaveData.GridSlotSaveData = _boardCoordinateSystem.GetGridSlotSaveData();
        gameSaveData.QueueSaveData = _queueController.GetQueueSaveData();
        gameSaveData.BusSaveData = _busController.GetBusSaveData();

        ES3.Save(_lastGameSave, gameSaveData, settings);
        ES3.StoreCachedFile();
    }

    public GameSaveData GetLastGameSave()
    {
        var gameSave = ES3.Load(_lastGameSave, (GameSaveData)null);
        ES3.DeleteKey(_lastGameSave);
        return gameSave;
    }

    public void Dispose()
    {
        _signalBus.TryUnsubscribe<GameStateChangedSignal>(OnGameStateChangedSignal);
    }
}

public class GameSaveData
{
    public GridSlotSaveData GridSlotSaveData;
    public List<QueueSaveData> QueueSaveData;
    public BusSaveData BusSaveData;
    public int LeftTime;
}

public class GridSlotSaveData
{
    public int RowCount;
    public int ColumnCount;
    public List<GridSlotData> GridSlotDatas;
}

public class QueueSaveData
{
    public bool IsEmpty;
    public PassengerType RegisteredPassengerType;
}

public class BusSaveData
{
    public List<BusData> BusDatas;
}