using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private Board _board;
    private List<Unit> _units;
    private List<Unit> _enemies;
    private int _actionDelay = 1000;

    public int ActionDelay
    {
        get { return _actionDelay; }
        set { _actionDelay = value; }
    }

    public static event Action<bool> AITurnEnded;

    void Start()
    {
        GameManager.TurnStarted += ResetActionDelay;
    }

    private void OnDisable()
    {
        GameManager.TurnStarted -= ResetActionDelay;
    }

    private void ResetActionDelay() {
        _actionDelay = 1000;
    }

    public void Init(Board board)
    {
        _board = board;
        _units = board.Units.Where(unit => unit.Faction == Faction.AI).OrderBy(unit => unit.AI.Priority).ToList();
        _enemies = board.Units.Where(unit => unit.Faction == Faction.Human).ToList();
    }

    public async void PlayTurn() {
        Debug.Log("Playing AI Turn!");
        _board.ClearHighlights();
        var activeUnits = _units.Where(unit => !unit.IsDead).ToList();

        foreach (var unit in activeUnits)
        {
            var moves = unit.Movement.CalculatePossibleMoves();
            _board.HighlightMoves(moves);
            var move = GetBestMove(unit, moves);

            await Task.Delay(_actionDelay);

            unit.Movement.TryToMove(move);
            if(unit.AI.Priority == 1 && unit.Tile.Position.y == 0)
            {
                AITurnEnded?.Invoke(true);
                break;
            }
            _board.ClearHighlights();
            if(!unit.Combat.CanAttack)
            {
                unit.SetUnitState(UnitState.Waiting);
                continue;
            }
            var targets = unit.Combat.CalculatePossibleTargets();
            _board.HighlightTargets(targets);

            if (targets.Count == 0)
            {
                unit.SetUnitState(UnitState.Waiting);
                await Task.Delay(_actionDelay);
                continue;
            }

            await Task.Delay(_actionDelay);

            var target = GetBestTarget(unit, targets);
            if(target != null)
            {
                unit.Combat.TryToAttack(target);
            }
            _board.ClearHighlights();
            await Task.Delay(_actionDelay);
        }

        AITurnEnded?.Invoke(false);
    }

    private Tile GetBestMove(Unit unit, List<Tile> tiles)
    {
        _enemies = _enemies.Where(enemy => !enemy.IsDead).ToList();
        switch(unit.AI.MovementBehaviour)
        {
            case MovementBehaviour.ClosestEnemy:
                var nearestEnemy = _enemies[0];
                var closestDistance = Vector2.Distance(unit.Tile.Position, nearestEnemy.Tile.Position);
                foreach (var enemy in _enemies)
                {
                    var newDistance = Vector2.Distance(unit.Tile.Position, enemy.Tile.Position);
                    if (newDistance < closestDistance)
                    {
                        nearestEnemy = enemy;
                        closestDistance = newDistance;
                    }
                }
                var bestMove = tiles.First();
                foreach (var move in tiles)
                {
                    if (Vector2.Distance(move.Position, nearestEnemy.Tile.Position) < Vector2.Distance(bestMove.Position, nearestEnemy.Tile.Position))
                    {
                        bestMove = move;
                    }
                }

                return bestMove;
            case MovementBehaviour.LowestThreat:

                var bestMoveTile = tiles.Select(tile => new KeyValuePair<Tile, int>(tile, _enemies.Where(enemy => enemy.Combat.CalculatePossibleTargets().Contains(tile)).Count()))
                                        .OrderBy(pair => pair.Value).First().Key;

                return bestMoveTile;
            case MovementBehaviour.Forward:
                tiles.Last();
                break;

        }
        return tiles.Last();
    }

    private Tile GetBestTarget(Unit unit, List<Tile> tiles)
    {
        var units = tiles.Select(tile => tile.Unit).ToList();
        var bestTarget = tiles.Count == 0 ? null : units.OrderBy(unit => unit.HP).ThenByDescending(unit => unit.Combat.AP).First().Tile;
        return bestTarget;
    }
}
