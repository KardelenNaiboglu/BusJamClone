using UnityEngine;

[CreateAssetMenu(menuName = "Settings/BoardSettings", fileName = "BoardSettings")]
public class BoardSettings : ScriptableObject
{
   public float GridSlotDistance;
   public float QueueSlotDistance;
   public int QueueSlotCount;
}
