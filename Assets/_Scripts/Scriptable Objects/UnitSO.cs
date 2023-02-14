using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit Scriptable Object")]
public class UnitSO : ScriptableObject
{
    public Faction Faction;
    public Unit UnitPrefab;

}