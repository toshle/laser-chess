using System.Linq;
using UnityEngine;

public class Reinforcement : EffectBase
{
    [SerializeField] UnitSO unit;

    public override void Activate(Unit triggerUnit)
    {
        if(triggerUnit.Faction == Faction.Human)
        {
            Debug.Log("Reinforcement " + unit.name);
            var freeTiles = _board.Tiles.Where(tile => tile.Unit == null && tile.Position.y < 2).ToList();
            if(freeTiles.Count > 0)
            {
                var index = Random.Range(0, freeTiles.Count);
                _board.SpawnUnit(unit, freeTiles[index]);
                Destroy(gameObject);
            }
        }
    }
}
