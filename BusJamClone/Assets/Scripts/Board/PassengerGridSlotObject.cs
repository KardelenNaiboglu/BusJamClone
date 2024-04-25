using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PassengerGridSlotObject : GridSlotObject, IPoolable<IMemoryPool>
{
    #region Zenject

    private PassengerSettings _passengerSettings;

    [Inject]
    private void Construct(PassengerSettings passengerSettings)
    {
        _passengerSettings = passengerSettings;
    }

    #endregion

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Outline outline;
    private IMemoryPool _pool;

    public PassengerType PassengerType { get; private set; }

    public void SetPassenger(PassengerType passengerType)
    {
        PassengerType = passengerType;
        SetView();
    }

    private void SetView()
    {
        var passengerData = _passengerSettings.GetPassengerDataByType(PassengerType);
        meshRenderer.material.color = passengerData.PassengerColor;
    }

    public void SetOutline(bool available)
    {
        outline.OutlineWidth = available ? 5f : 0f;
    }

    public override void GoToPool()
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

    public class Factory : PlaceholderFactory<PassengerGridSlotObject>
    {
    }

    public class Pool : MonoPoolableMemoryPool<IMemoryPool, PassengerGridSlotObject>
    {
    }
}