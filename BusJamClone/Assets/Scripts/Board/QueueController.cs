using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class QueueController : IInitializable, IDisposable
{
    #region Zenject

    private BoardSettings _boardSettings;
    private QueueSlot.Factory _queueSlotFactory;
    private SceneReferenceHolder _sceneReferenceHolder;
    private BusController _busController;

    [Inject]
    private void Construct(BoardSettings boardSettings,
        QueueSlot.Factory queueSlotFactory,
        SceneReferenceHolder sceneReferenceHolder,
        BusController busController)
    {
        _boardSettings = boardSettings;
        _queueSlotFactory = queueSlotFactory;
        _sceneReferenceHolder = sceneReferenceHolder;
        _busController = busController;
    }

    #endregion

    private List<QueueSlot> _queueSlots;

    public void Initialize()
    {
    }

    public void SpawnQueue(List<QueueSaveData> queueSaveDatas = null)
    {
        if (_queueSlots != null)
        {
            foreach (var slot in _queueSlots)
            {
                slot.GoToPool();
            }

            _queueSlots.Clear();
        }

        _queueSlots = new List<QueueSlot>();

        for (int i = 0; i < _boardSettings.QueueSlotCount; i++)
        {
            var queueSlot = _queueSlotFactory.Create();
            queueSlot.SetIndex(i);
            queueSlot.transform.parent = _sceneReferenceHolder.QueueHolder;
            queueSlot.transform.localPosition = Vector3.right * (i - _boardSettings.QueueSlotCount / 2f +
                                                                 (_boardSettings.QueueSlotCount % 2 == 0 ? 0.5f : 0f)) *
                                                _boardSettings.QueueSlotDistance;
            _queueSlots.Add(queueSlot);

            if (queueSaveDatas != null)
            {
                queueSlot.ResolveSaveData(queueSaveDatas[i]);
            }
        }
    }


    public void ProcessPassengerAtGrid(GridSlot gridSlot)
    {
        if (gridSlot == null) return;

        var passenger = gridSlot.GetPassenger();

        if (passenger == null) return;

        gridSlot.UnregisterPassenger();

        if (!_busController.HasBusAvailable() || !_busController.NeedsPassengerType(passenger.PassengerType))
        {
            AddPassengerToQueue(passenger);
        }
        else
        {
            TravelPassengerToBus(passenger);
        }
    }

    private void AddPassengerToQueue(PassengerGridSlotObject passenger)
    {
        var emptySlot = _queueSlots.First(t => t.IsEmpty());

        emptySlot.SetPassenger(passenger);
    }

    private void TravelPassengerToBus(PassengerGridSlotObject passenger)
    {
        _busController.TravelPassenger(passenger);
    }

    public void SendReadyPassengersToBus()
    {
        foreach (var queueSlot in _queueSlots)
        {
            if (queueSlot.IsEmpty()) continue;
            if (_busController.HasBusAvailable() &&
                _busController.NeedsPassengerType(queueSlot.RegisteredPassenger.PassengerType))
            {
                TravelPassengerToBus(queueSlot.RegisteredPassenger);
                queueSlot.UnregisterPassenger();
            }
        }
    }

    public bool HasEmptySlot()
    {
        return _queueSlots.Any(slot => slot.IsEmpty());
    }

    public List<QueueSaveData> GetQueueSaveData()
    {
        List<QueueSaveData> queueSaveDatas = new List<QueueSaveData>();

        for (int i = 0; i < _queueSlots.Count; i++)
        {
            queueSaveDatas.Add(new QueueSaveData()
            {
                IsEmpty = _queueSlots[i].IsEmpty(),
                RegisteredPassengerType = _queueSlots[i].RegisteredPassenger != null
                    ? _queueSlots[i].RegisteredPassenger.PassengerType
                    : PassengerType.Red,
            });
        }

        return queueSaveDatas;
    }

    public void Dispose()
    {
    }
}