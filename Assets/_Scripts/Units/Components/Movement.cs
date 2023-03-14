using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private int _speed;
    [SerializeField] private DirectionType _movementType;
    [SerializeField] private Tile _targetTile;
    [SerializeField] private List<Vector2> _movementPattern;
    [SerializeField] private int _movementSpeed = 10;
    private List<Tile> _possibleMoves = new();

    public List<Tile> PossibleMoves => _possibleMoves;

    public static event Action<Unit, Tile, bool> PredictedAttacks;


    private void Start()
    {
        Unit.ShowPossibleMoves += OnShowPossibleMoves;
        Unit.UnitMoves += OnUnitMoves;
        Tile.TileHovered += OnTileHovered;
    }
    private void OnDisable()
    {
        Unit.ShowPossibleMoves -= OnShowPossibleMoves;
        Unit.UnitMoves -= OnUnitMoves;
        Tile.TileHovered -= OnTileHovered;
    }
    private void Update()
    {
        if (_targetTile != null)
        {
            if (transform.position != _targetTile.transform.position)
            {
                var target = _targetTile.transform.position + new Vector3(0, 0.5f, 0);
                transform.position = Vector3.Lerp(transform.position, target, _movementSpeed * Time.deltaTime);
            } else
            {
                _targetTile = null;
            }
        }
    }


    private void OnTileHovered(Tile tile, bool isHovered)
    {
        if (_unit.IsSelected && _unit.State == UnitState.Moving)
        {
            if (_possibleMoves.Contains(tile))
            {
                if (isHovered)
                {
                    PredictedAttacks?.Invoke(_unit, tile, true);
                }
                else
                {
                    PredictedAttacks?.Invoke(_unit, tile, false);
                }
            }
        }
    }

    private void OnUnitMoves(Unit unit, Tile tile)
    {
        if(unit == _unit)
        {
            //Debug.Log("I want to move here: " + tile);
            TryToMove(tile);
        }
    }

    private void OnShowPossibleMoves(Unit unit, Tile tile)
    {
        if (unit == _unit)
        {
            var moves = CalculatePossibleMoves();
            tile.Board.HighlightMoves(moves);
            PredictedAttacks?.Invoke(_unit, tile, true);
        }
    }

    public void TryToMove(Tile tile)
    {
        if (_possibleMoves.Contains(tile))
        {
            MoveTo(tile);
            _unit.SetUnitState(UnitState.Attacking);
        }
    }

    public void MoveTo(Tile tile)
    {
        Debug.Log(_unit + " Moving to " + tile);
        _targetTile = tile;
        _unit.Tile.Unit = null;
        tile.Unit = _unit;
        _unit.Tile = tile;
        var target = _targetTile.transform.position + new Vector3(0, 0.5f, 0);
        _unit.transform.LookAt(target, Vector3.up);
        if(tile.Effect != null)
        {
            tile.Effect.Activate();
        }
    }

    public List<Tile> CalculatePossibleMoves()
    {
        _possibleMoves.Clear();
        var possibleMoves = new List<Tile>
        {
            _unit.Tile
        };
        switch (_movementType)
        {
            case DirectionType.All:
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(0, 1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(0, -1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(1, 0)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(-1, 0)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(1, 1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(-1, 1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(1, -1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(-1, -1)));
                break;
            case DirectionType.Cross:
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(0, 1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(0, -1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(1, 0)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(-1, 0)));
                break;
            case DirectionType.X:
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(1, 1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(-1, 1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(1, -1)));
                possibleMoves.AddRange(GetMovesInDirection(new Vector2(-1, -1)));
                break;
            case DirectionType.Pattern:
                foreach(var move in _movementPattern)
                {
                    var tile = _unit.Tile.Board.GetTileAtPosition(move + _unit.Tile.Position);
                    if(tile != null && (tile.Unit == null || tile.Unit.IsDead))
                    {
                        possibleMoves.Add(tile);
                    }
                }
                break;
            default: break;
        }

        _possibleMoves.AddRange(possibleMoves);

        return possibleMoves;
    }

    private List<Tile> GetMovesInDirection(Vector2 direction)
    {
        var tiles = _unit.Tile.Board.GetTilesInDirection(_unit.Tile, direction, _speed);
        var moves = new List<Tile>();

        foreach (var tile in tiles)
        {
            if (tile.Unit == null || tile.Unit.IsDead)
            {
                moves.Add(tile);
            } else
            {
                break;
            }
        }

        return moves;
    }
}