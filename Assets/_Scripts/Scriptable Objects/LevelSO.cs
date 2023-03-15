using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "New Level", menuName = "Level Scriptable Object")]
public class LevelSO : ScriptableObject
{
    public AssetReference reference;
    public string sceneName;
    public bool IsLocked;
}