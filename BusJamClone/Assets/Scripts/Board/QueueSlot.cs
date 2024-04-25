using UnityEngine;
using Zenject;

public class QueueSlot : MonoBehaviour, IPoolable<IMemoryPool>
{
    #region Zenject

    private LevelController _levelController;

    [Inject]
    private void Construct(LevelController levelController)
    {
        _levelController = levelController;
    }

    #endregion

    public int Index { get; private set; }
    private IMemoryPool _pool;
    public PassengerGridSlotObject RegisteredPassenger { get; private set; }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public bool IsEmpty()
    {
        return RegisteredPassenger == null;
    }

    public void SetPassenger(PassengerGridSlotObject passenger)
    {
        RegisteredPassenger = passenger;
        RegisteredPassenger.transform.parent = transform;
        RegisteredPassenger.transform.localPosition = Vector3.zero;
    }

    public void UnregisterPassenger()
    {
        RegisteredPassenger = null;
    }

    public void ResolveSaveData(QueueSaveData queueSaveData)
    {
        if (!queueSaveData.IsEmpty)
        {
            SetPassenger((PassengerGridSlotObject)_levelController.GetGridObjectForGrid(GridSlotObjectType.Passenger,
                queueSaveData.RegisteredPassengerType));
        }
    }

    public void GoToPool()
    {
        if (RegisteredPassenger != null)
        {
            RegisteredPassenger.GoToPool();
            RegisteredPassenger = null;
        }

        _pool.Despawn(this);
    }

    public void OnDespawned()
    {
    }

    public void OnSpawned(IMemoryPool pool)
    {
        _pool = pool;
    }

    public class Factory : PlaceholderFactory<QueueSlot>
    {
    }

    public class Pool : MonoPoolableMemoryPool<IMemoryPool, QueueSlot>
    {
    }
}