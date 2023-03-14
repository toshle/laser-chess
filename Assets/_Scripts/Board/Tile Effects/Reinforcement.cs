using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reinforcement : EffectBase
{
    [SerializeField] UnitSO unit;

    public override void Activate()
    {
        if(unit.Faction == Faction.Human)
        {
            Debug.Log("Reinforcement " + unit.name);
            var freeHumanTiles = _board.Tiles.Where(tile => tile.Unit == null && tile.Position.y < 3).ToList();
            var index = UnityEngine.Random.Range(0, freeHumanTiles.Count);
            _board.SpawnUnit(unit, freeHumanTiles[index]);
            Destroy(gameObject);
        }
    }
}
