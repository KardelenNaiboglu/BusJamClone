using UnityEngine;

[CreateAssetMenu(menuName = "Settings/ReferenceHolder", fileName = "ReferenceHolder")]
public class ReferenceHolder : ScriptableObject
{
    public GridSlot GridSlotPrefab;
    public PassengerGridSlotObject PassengerPrefab;
    public BusObject BusPrefab;
    public QueueSlot QueueSlotPrefab;
}