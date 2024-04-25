using Zenject;

public class LevelController
{
    #region Zenject

    private SaveController _saveController;
    private LevelSettings _levelSettings;
    private BusController _busController;
    private TimerController _timerController;
    private PassengerGridSlotObject.Factory _passengerFactory;
    private BoardCoordinateSystem _boardCoordinateSystem;
    private QueueController _queueController;
    private SignalBus _signalBus;

    [Inject]
    private void Construct(SaveController saveController,
        LevelSettings levelSettings,
        BusController busController,
        TimerController timerController,
        PassengerGridSlotObject.Factory passengerFactory,
        BoardCoordinateSystem boardCoordinateSystem,
        QueueController queueController,
        SignalBus signalBus)
    {
        _saveController = saveController;
        _levelSettings = levelSettings;
        _busController = busController;
        _timerController = timerController;
        _passengerFactory = passengerFactory;
        _boardCoordinateSystem = boardCoordinateSystem;
        _queueController = queueController;
        _signalBus = signalBus;
    }

    #endregion

    private bool _isSavedGameLoaded = false;

    public void SpawnLevel()
    {
        var levelIndex = _saveController.GetCurrentLevelIndex();

        var lastGameSave = _saveController.GetLastGameSave();

        if (lastGameSave != null)
        {
            _isSavedGameLoaded = true;
            _busController.CreateBuses(lastGameSave.BusSaveData.BusDatas);
            _timerController.SetTimer(lastGameSave.LeftTime);
            _boardCoordinateSystem.SpawnGridSlotsBySave(lastGameSave.GridSlotSaveData);
            _queueController.SpawnQueue(lastGameSave.QueueSaveData);
        }
        else
        {
            _isSavedGameLoaded = false;
            var currentLevel = _levelSettings.Levels.Count <= levelIndex
                ? _levelSettings.Levels[^1]
                : _levelSettings.Levels[levelIndex];
            _busController.CreateBuses(currentLevel.BusDatas);
            _timerController.SetTimer(currentLevel.TimeInSeconds);
            _boardCoordinateSystem.SpawnGridSlots(currentLevel);
            _queueController.SpawnQueue();
        }
    }

    public GridSlotObject GetGridObjectForGrid(GridSlotObjectType gridSlotObjectType, PassengerType passengerType)
    {
        switch (gridSlotObjectType)
        {
            case GridSlotObjectType.Passenger:
                var passenger = _passengerFactory.Create();
                passenger.SetPassenger(passengerType);
                return passenger;

            default: return null;
        }
    }

    public void SavedGameCheck()
    {
        if (_isSavedGameLoaded) _signalBus.Fire<SavedGameLoadedSignal>();
    }
}