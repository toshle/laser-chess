using System;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private int _ap;
    [SerializeField] private int _range = 1;
    [SerializeField] private bool _canAttackSimultaneously;
    [SerializeField] private bool _canAttack = true;
    [SerializeField] private DirectionType _attackDirection;

    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private ParticleSystem _damagePrefab;
    [SerializeField] private ParticleSystem _deathPrefab;

    private List<Tile> _possibleTargets = new();
    public bool CanAttack => _canAttack;

    public int AP => _ap;
    public List<Tile> PossibleTargets => _possibleTargets;

    public static event Action<Unit> UnitDied;
    public static event Action UnitAttacked;

    private void Start()
    {
        Unit.ShowPossibleTargets += OnShowPossibleTargets;
        Unit.UnitAttacks += OnUnitAttacks;
        Movement.PredictedAttacks += OnPredictedAttacks;
    }
    private void OnDisable()
    {
        Unit.ShowPossibleTargets -= OnShowPossibleTargets;
        Unit.UnitAttacks -= OnUnitAttacks;
        Movement.PredictedAttacks -= OnPredictedAttacks;
    }

    private void OnPredictedAttacks(Unit unit, Tile tile, bool isPredicting)
    {
        if (unit == _unit && _canAttack)
        {
            var targets = CalculatePossibleTargets(tile);
            if (isPredicting)
            {
                unit.Tile.Board.HighlightTargets(targets);
            } else
            {
                unit.Tile.Board.ClearHighlights(targets);
            }
        }
    }

    private void OnUnitAttacks(Unit unit, Tile tile)
    {
        if (unit == _unit)
        {
            //Debug.Log(unit + ": I want to attack this tile: " + tile);
            TryToAttack(tile);
        }
    }

    private void OnShowPossibleTargets(Unit unit, Tile tile)
    {
        if (unit == _unit)
        {
            var targets = CalculatePossibleTargets();
            tile.Board.HighlightTargets(targets);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log(_unit + " Takes damage: " + damage);
        _unit.HP = _unit.HP - damage;
        Instantiate(_damagePrefab, _unit.transform.position, Quaternion.identity);

        if (_unit.HP <= 0)
        {

            Debug.Log(_unit + " was destroyed!");
            _unit.IsDead = true;
            _unit.gameObject.SetActive(false);
            Instantiate(_deathPrefab, _unit.transform.position, Quaternion.identity);
            UnitDied?.Invoke(_unit);
        }
    }

    public void TryToAttack(Tile tile)
    {   
        if(!_canAttack)
        {
            _unit.SetUnitState(UnitState.Waiting);
            return;
        }

        if (_possibleTargets.Contains(tile))
        {
            if (_canAttackSimultaneously)
            {
                Debug.Log(_unit + ": I am attacking all my targets");
                foreach (var target in _possibleTargets)
                {
                    Attack(target.Unit);
                }
            }
            else
            {
                Debug.Log(_unit + ": I am attacking the unit on this tile : " + tile);
                Attack(tile.Unit);
            }
            _unit.SetUnitState(UnitState.Waiting);
        }
    }

    private void Attack(Unit unit)
    {
        var projectile = Instantiate(_projectilePrefab, _unit.transform.position, Quaternion.identity);
        projectile.Target = unit.transform.position;
        _unit.transform.LookAt(unit.transform);
        Debug.Log(_unit + " Attacks unit: " + unit);
        unit.Combat.TakeDamage(_ap);
        UnitAttacked?.Invoke();
    }

    public List<Tile> CalculatePossibleTargets(Tile sourceTile = null)
    {
        //Debug.Log("Calculating possible targets for: " + _unit);
        _possibleTargets.Clear();
        var possibleTargets = new List<Tile>();
        switch (_attackDirection)
        {
            case DirectionType.All:
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(0, 1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(0, -1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(1, 1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(1, -1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(1, 0)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(-1, 0)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(-1, 1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(-1, -1)));
                break;
            case DirectionType.Cross:
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(-1, 0)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(1, 0)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(0, 1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(0, -1)));
                break;
            case DirectionType.X:
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(-1, 1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(1, 1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(1, -1)));
                possibleTargets.AddRange(GetTargetInDirection(sourceTile, new Vector2(-1, -1)));
                break;
            default: break;
        }

        _possibleTargets.AddRange(possibleTargets);
        //Debug.Log("Targets found: " + _possibleTargets.Count);
        if(_unit.State == UnitState.Attacking && _possibleTargets.Count == 0)
        {
            _unit.SetUnitState(UnitState.Waiting);
        }
        
        return possibleTargets;
    }

    private List<Tile> GetTargetInDirection(Tile sourceTile, Vector2 direction) {
        var source = sourceTile != null ? sourceTile : _unit.Tile;
        var tiles = source.Board.GetTilesInDirection(source, direction, _range);
        List<Tile> target = new();

        foreach (var tile in tiles)
        {
            if (tile.Unit != null && !tile.Unit.IsDead)
            {
                if (tile.Unit.Faction != _unit.Faction)
                {
                    target.Add(tile);
                }
                break;
            }
        }

        return target;
    }
}