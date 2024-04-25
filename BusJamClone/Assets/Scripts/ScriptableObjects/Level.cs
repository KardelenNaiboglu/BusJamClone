using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Level", fileName = "Level")]
public class Level : ScriptableObject
{
    public int TimeInSeconds;
    public int RowCount;
    public int ColumnCount;
    public List<GridSlotData> GridSlotDatas;
    public List<BusData> BusDatas;
}

[Serializable]
public class BusData
{
    public PassengerType PassengerType;
    [HideInInspector] public int FilledCapacity;
}

[Serializable]
public class GridSlotData
{
    public GridSlotState GridSlotState;

    public GridSlotObjectType GridSlotObjectType;

    public PassengerType PassengerType;
}