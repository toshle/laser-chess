using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Scriptable Object")]
public class LevelSO : ScriptableObject
{
    public List<UnitSpawnPoint> Units;
    public int Width, Height;
    public bool IsLocked;
}