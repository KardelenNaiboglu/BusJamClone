using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/PassengerSettings", fileName = "PassengerSettings")]
public class PassengerSettings : ScriptableObject
{
    public List<PassengerData> PassengerDatas;

    public PassengerData GetPassengerDataByType(PassengerType passengerType)
    {
        return PassengerDatas.FirstOrDefault(t => t.PassengerType == passengerType);
    }
}

[Serializable]
public class PassengerData
{
    public PassengerType PassengerType;
    public Color PassengerColor;
}