using UnityEngine;
using Zenject;

public class GridSlot : MonoBehaviour, IPoolable<IMemoryPool>
{
    #region Zenject

    private LevelController _levelController;

    [Inject]
    private void Construct(LevelController levelController)
    {
        _levelController = levelController;
    }

    #endregion

    [SerializeField] private Transform gridSlotVisual;
    private IMemoryPool _pool;
    private GridSlotData _gridSlotData;
    private GridSlotObject _gridSlotObject;
    private bool _isAvailable = false;

    public int RowIndex { get; private set; }
    public int ColumnIndex { get; private set; }

    public void SetGridSlot(GridSlotData gridSlotData)
    {
        _gridSlotData = new GridSlotData()
        {
            GridSlotState = gridSlotData.GridSlotState,
            GridSlotObjectType = gridSlotData.GridSlotObjectType,
            PassengerType = gridSlotData.PassengerType
        };

        SetView();
    }

    public void SetCoordinates(int row, int column)
    {
        RowIndex = row;
        ColumnIndex = column;
    }

    public GridSlotState GetGridSlotState()
    {
        return _gridSlotData.GridSlotState;
    }

    public bool IsAvailableToInteract()
    {
        return _gridSlotObject != null && _isAvailable;
    }

    public PassengerGridSlotObject GetPassenger()
    {
        return (PassengerGridSlotObject)_gridSlotObject;
    }

    public void UnregisterPassenger()
    {
        ((PassengerGridSlotObject)_gridSlotObject).SetOutline(false);
        _gridSlotObject = null;
        _gridSlotData.GridSlotState = GridSlotState.Empty;
    }

    public void SetAvailability(bool available)
    {
        _isAvailable = available;
        if (_gridSlotObject != null)
        {
            ((PassengerGridSlotObject)_gridSlotObject).SetOutline(available);
        }
    }

    private void SetView()
    {
        switch (_gridSlotData.GridSlotState)
        {
            case GridSlotState.Empty:
                gridSlotVisual.gameObject.SetActive(true);
                break;
            case GridSlotState.Disabled:
                gridSlotVisual.gameObject.SetActive(false);
                break;
            case GridSlotState.Occupied:
                gridSlotVisual.gameObject.SetActive(true);
                SetGridSlotObject(_levelController.GetGridObjectForGrid(_gridSlotData.GridSlotObjectType,
                    _gridSlotData.PassengerType));
                break;

            default: break;
        }
    }

    private void SetGridSlotObject(GridSlotObject gridSlotObject)
    {
        if (gridSlotObject == null)
        {
            Debug.Log("Grid slot object is null.");
            return;
        }

        _gridSlotObject = gridSlotObject;
        _gridSlotObject.transform.parent = transform;
        _gridSlotObject.transform.localPosition = Vector3.zero;
    }

    public GridSlotData GetSaveData()
    {
        return _gridSlotData;
    }

    public void GoToPool()
    {
        if (_gridSlotObject != null)
        {
            _gridSlotObject.GoToPool();
            _gridSlotObject = null;
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

    public class Factory : PlaceholderFactory<GridSlot>
    {
    }

    public class Pool : MonoPoolableMemoryPool<IMemoryPool, GridSlot>
    {
    }
}