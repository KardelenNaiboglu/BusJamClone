using UnityEngine;

public class EditorPassenger : MonoBehaviour
{
    public PassengerType PassengerType { get; private set; }

    [SerializeField] private MeshRenderer meshRenderer;

    public void SetPassengerTypeAndColor(PassengerData passengerData)
    {
        PassengerType = passengerData.PassengerType;
        meshRenderer.material.color = passengerData.PassengerColor;
    }
}