using TMPro;
using UnityEngine;
using Zenject;

public class BusObject : MonoBehaviour, IPoolable<IMemoryPool>
{
    #region Zenject

    private PassengerSettings _passengerSettings;
    private BusController _busController;

    [Inject]
    private void Construct(PassengerSettings passengerSettings,
        BusController busController)
    {
        _passengerSettings = passengerSettings;
        _busController = busController;
    }

    #endregion

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshPro capacityText;

    public PassengerType PassengerType { get; private set; }

    private IMemoryPool _pool;

    public BusData BusData { get; private set; }

    public void SetBus(BusData busData)
    {
        BusData = new BusData()
        {
            PassengerType = busData.PassengerType,
            FilledCapacity = busData.FilledCapacity,
        };

        PassengerType = BusData.PassengerType;

        SetView();
    }

    private void SetView()
    {
        var passengerData = _passengerSettings.GetPassengerDataByType(BusData.PassengerType);
        meshRenderer.material.color = passengerData.PassengerColor;
        UpdateCapacityText();
    }

    public bool IsAvailable()
    {
        return true;
    }

    public bool IsFilled()
    {
        if (BusData == null) return false;

        return BusData.FilledCapacity == 3;
    }

    public void TakePassenger(PassengerGridSlotObject passenger)
    {
        BusData.FilledCapacity++;
        UpdateCapacityText();
        passenger.GoToPool();

        if (IsFilled())
        {
            _busController.BusFilled();
        }
    }

    private void UpdateCapacityText()
    {
        if (BusData != null) capacityText.text = $" {BusData.FilledCapacity} / 3 ";
    }

    public void GoToPool()
    {
        _pool.Despawn(this);
    }

    public void OnDespawned()
    {
    }

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
    }

    public class Factory : PlaceholderFactory<BusObject>
    {
    }

    public class Pool : MonoPoolableMemoryPool<IMemoryPool, BusObject>
    {
    }
}