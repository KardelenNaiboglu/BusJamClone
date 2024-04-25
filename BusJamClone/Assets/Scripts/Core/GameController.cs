using System;
using UnityEngine;
using Zenject;

public class GameController : IInitializable, IDisposable
{
    #region Zenject

    private LevelController _levelController;
    private SignalBus _signalBus;
    private Camera _camera;
    private UIManager _uiManager;
    private QueueController _queueController;
    private BoardCoordinateSystem _boardCoordinateSystem;

    [Inject]
    private void Construct(LevelController levelController,
        SignalBus signalBus,
        Camera camera,
        UIManager uiManager,
        QueueController queueController,
        BoardCoordinateSystem boardCoordinateSystem)
    {
        _levelController = levelController;
        _signalBus = signalBus;
        _camera = camera;
        _uiManager = uiManager;
        _queueController = queueController;
        _boardCoordinateSystem = boardCoordinateSystem;
    }

    #endregion

    public GameState GameState => _gameState;
    private GameState _gameState;

    private int _layerMaskGridSlot;

    public void Initialize()
    {
        _layerMaskGridSlot = 1 << LayerMask.NameToLayer("GridSlot");
        _uiManager.Init();

        _signalBus.Subscribe<FingerDownSignal>(OnFingerDownSignal);
        _signalBus.Subscribe<TimesUpSignal>(OnTimesUpSignal);
        _signalBus.Subscribe<SavedGameLoadedSignal>(OnSavedGameLoadedSignal);

        SpawnLevel();
    }

    private void OnSavedGameLoadedSignal()
    {
        SetState(GameState.Playing);
    }

    private void SpawnLevel()
    {
        SetState(GameState.Initializing);
        _levelController.SpawnLevel();
        _boardCoordinateSystem.UpdateGridSlotAvailabilities();
        SetState(GameState.ReadyToStart);
        _levelController.SavedGameCheck();
    }

    private void OnFingerDownSignal(FingerDownSignal signal)
    {
        HandleInput(signal.Position);
    }

    private void OnTimesUpSignal()
    {
        SetState(GameState.Fail);
    }

    private void HandleInput(Vector3 position)
    {
        switch (_gameState)
        {
            case GameState.ReadyToStart:
                SetState(GameState.Playing);
                RaycastForGridObjectInteraction(position);
                break;

            case GameState.Playing:
                RaycastForGridObjectInteraction(position);
                break;

            default:
                break;
        }
    }

    private void SetState(GameState gameState)
    {
        _gameState = gameState;
        _signalBus.Fire(new GameStateChangedSignal(_gameState));
    }

    private void RaycastForGridObjectInteraction(Vector3 position)
    {
        if (!_queueController.HasEmptySlot()) return;
        if (Physics.Raycast(_camera.ScreenPointToRay(position), out var hit, 200, _layerMaskGridSlot))
        {
            if (hit.collider.TryGetComponent(out GridSlot gridSlot))
            {
                if (gridSlot.IsAvailableToInteract())
                {
                    _queueController.ProcessPassengerAtGrid(gridSlot);
                    _boardCoordinateSystem.UpdateGridSlotAvailabilities();
                }
            }
        }
    }

    public void ButtonClickPlay()
    {
        SpawnLevel();
    }

    public void LevelWin()
    {
        SetState(GameState.Win);
    }

    public void LevelLose()
    {
    }

    public void Dispose()
    {
        _uiManager.Dispose();
        _signalBus.TryUnsubscribe<FingerDownSignal>(OnFingerDownSignal);
        _signalBus.TryUnsubscribe<TimesUpSignal>(OnTimesUpSignal);
        _signalBus.TryUnsubscribe<SavedGameLoadedSignal>(OnSavedGameLoadedSignal);
    }
}