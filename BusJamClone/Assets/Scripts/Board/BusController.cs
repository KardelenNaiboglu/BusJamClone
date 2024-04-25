using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BusController : IInitializable, IDisposable
{
    #region Zenject

    private BusObject.Factory _busFactory;
    private SceneReferenceHolder _sceneReferenceHolder;
    private QueueController _queueController;
    private GameController _gameController;

    [Inject]
    private void Construct(BusObject.Factory busFactory,
        SceneReferenceHolder sceneReferenceHolder,
        QueueController queueController,
        GameController gameController)
    {
        _busFactory = busFactory;
        _sceneReferenceHolder = sceneReferenceHolder;
        _queueController = queueController;
        _gameController = gameController;
    }

    #endregion

    private List<BusObject> _buses = new();
    private int _currentBusIndex;

    public void Initialize()
    {
    }

    public void CreateBuses(List<BusData> currentLevelBusDatas)
    {
        _currentBusIndex = 0;

        if (_buses != null)
        {
            foreach (var bus in _buses)
            {
                bus.GoToPool();
            }

            _buses.Clear();
        }

        _buses = new List<BusObject>();

        for (int i = 0; i < currentLevelBusDatas.Count; i++)
        {
            var bus = CreateBusObject();
            bus.SetBus(currentLevelBusDatas[i]);
            bus.transform.parent = _sceneReferenceHolder.BusHolder;
            _buses.Add(bus);

            if (_buses[_currentBusIndex].IsFilled())
            {
                _currentBusIndex = i;
            }
        }

        for (int i = 0; i < _buses.Count; i++)
        {
            _buses[i].transform.localPosition = Vector3.left * i * 2 + Vector3.right * _currentBusIndex * 2;
        }
    }

    public bool HasBusAvailable()
    {
        if (_currentBusIndex >= _buses.Count) return false;

        return !_buses[_currentBusIndex].IsFilled() && _buses[_currentBusIndex].IsAvailable();
    }

    public bool NeedsPassengerType(PassengerType passengerType)
    {
        if (_currentBusIndex >= _buses.Count) return false;

        return _buses[_currentBusIndex].PassengerType == passengerType;
    }

    public void TravelPassenger(PassengerGridSlotObject passenger)
    {
        _buses[_currentBusIndex].TakePassenger(passenger);
    }

    private BusObject CreateBusObject()
    {
        return _busFactory.Create();
    }

    public void BusFilled()
    {
        _currentBusIndex++;
        MoveBuses();

        if (!HasBusAvailable())
        {
            _gameController.LevelWin();
        }
    }

    private void MoveBuses()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var bus in _buses)
        {
            sequence.Join(bus.transform.DOLocalMoveX(bus.transform.localPosition.x + 2, 0.5f));
        }

        sequence.onComplete = CheckForOtherPassengers;
    }

    private void CheckForOtherPassengers()
    {
        _queueController.SendReadyPassengersToBus();
    }

    public BusSaveData GetBusSaveData()
    {
        BusSaveData busSaveData = new BusSaveData();
        busSaveData.BusDatas = new List<BusData>();

        for (int i = 0; i < _buses.Count; i++)
        {
            busSaveData.BusDatas.Add(_buses[i].BusData);
        }

        return busSaveData;
    }

    public void Dispose()
    {
    }
}