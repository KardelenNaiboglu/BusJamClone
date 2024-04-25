using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/LevelSettings", fileName = "LevelSettings")]
public class LevelSettings : ScriptableObject
{
    public List<Level> Levels;
}