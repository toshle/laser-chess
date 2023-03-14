using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Scriptable Object")]
public class LevelSO : ScriptableObject
{
    public string sceneName;
    public bool IsLocked;
}