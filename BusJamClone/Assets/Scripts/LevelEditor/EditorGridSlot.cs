using UnityEngine;

public class EditorGridSlot : MonoBehaviour
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public GridSlotState State { get; private set; }
    private EditorPassenger _registeredPassenger;
    [SerializeField] private MeshRenderer gridTileMeshRenderer;

    public void SetCoordinates(int i, int j)
    {
        Row = i;
        Column = j;
    }

    public GridSlotData GetGridSlotData()
    {
        return new GridSlotData()
        {
            GridSlotState = State,
            GridSlotObjectType = GridSlotObjectType.Passenger,
            PassengerType = _registeredPassenger != null ? _registeredPassenger.PassengerType : PassengerType.Red,
        };
    }

    public void SetGridSlotState(GridSlotState value)
    {
        State = value;
        SetGridView();
    }

    public void SetPassenger(EditorPassenger createdPassenger)
    {
        SetGridSlotState(GridSlotState.Occupied);

        if (_registeredPassenger != null)
        {
            Destroy(_registeredPassenger.gameObject);
        }

        _registeredPassenger = createdPassenger;
        _registeredPassenger.transform.parent = transform;
        _registeredPassenger.transform.localPosition = Vector3.zero;
    }

    private void SetGridView()
    {
        switch (State)
        {
            case GridSlotState.Disabled:
                gridTileMeshRenderer.material.color = Color.grey;
                break;

            default:
                gridTileMeshRenderer.material.color = Color.white;
                break;
        }
    }
}