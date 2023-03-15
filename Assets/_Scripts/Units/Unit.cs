using System;
using UnityEditor;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int _maxHP;
    [SerializeField] private int _HP;
    [SerializeField] private string _name;
    [SerializeField] private Board _board;
    [SerializeField] private Tile _tile;
    [SerializeField] private Movement _movement;
    [SerializeField] private Combat _combat;
    [SerializeField] private AI _ai;
    [SerializeField] private UnitState _state;
    [SerializeField] private Faction _faction;
    [SerializeField] private int _order;

    [SerializeField] private bool _isSelected = false;
    [SerializeField] private bool _hasMoved = false;
    [SerializeField] private bool _hasAttacked = false;
    [SerializeField] private bool _isDead = false;

    public bool IsSelected  => _isSelected;
    public Tile Tile
    {
        get { return _tile; }
        set { _tile = value; }
    }
    public int HP
    {
        get { return _HP; }
        set { _HP = value; }
    }

    public int MaxHP => _maxHP;
    public string Name => _name;
    public int Order => _order;
    public UnitState State => _state;
    public Movement Movement => _movement;
    public Combat Combat => _combat;
    public AI AI => _ai;
    public Board Board
    {
        get { return _board; }
        set { _board = value; }
    }
    public bool IsDead {
        get { return _isDead; }
        set { _isDead = value; }
    }
    public bool HasMoved
    {
        get { return _hasMoved; }
        set { _hasMoved = value; }
    }
    public bool HasAttacked
    {
        get { return _hasAttacked; }
        set { _hasAttacked = value; }
    }

    public Faction Faction => _faction;

    public static event Action<Unit, Tile> UnitMoves;
    public static event Action<Unit, Tile> UnitAttacks;
    public static event Action<Unit, Tile> ShowPossibleMoves;
    public static event Action<Unit, Tile> ShowPossibleTargets;

    private void Start()
    {
        Tile.TileSelected += OnTileSelected;
        GameManager.TurnStarted += OnTurnStarted;
        Init();
    }
    private void OnDisable()
    {
        Tile.TileSelected -= OnTileSelected;
        GameManager.TurnStarted -= OnTurnStarted;
    }

    public void Init()
    {
        var x = (int)Math.Round(Math.Abs(transform.localPosition.x));
        var y = (int)Math.Round(Math.Abs(transform.localPosition.z));
        var tile = _board.GetTileAtPosition(new Vector2(x, y));
        _tile = tile;
        tile.Unit = this;
        _board.AddUnit(this);
        _movement.MoveTo(tile);
    }

    private void OnTurnStarted()
    {
        SetSelected(false);
        SetUnitState(UnitState.Moving);
    }

    private void OnTileSelected(Tile tile) {
        if (_isSelected)
        {
            if (_state == UnitState.Moving)
            {
                UnitMoves?.Invoke(this, tile);
            } else if (_state == UnitState.Attacking) {
                UnitAttacks?.Invoke(this, tile);
            }
        }

        if (tile.Unit == this && _faction != Faction.AI)
        {
            SetSelected(true);
        }
        else
        {
            SetSelected(false);
        }
    }

    public void SetSelected(bool value) {
        _isSelected = value;
        if (_isSelected)
        {
            Debug.Log("Selected unit: " + this);
            if(_state == UnitState.Moving)
            {
                ShowPossibleMoves?.Invoke(this, _tile);
            }
            if (_state == UnitState.Attacking)
            {
                ShowPossibleTargets?.Invoke(this, _tile);
            }
        }
    }

    public void SetUnitState(UnitState state)
    {
        _state = state;
        switch (_state)
        {
            case UnitState.Moving:
                _tile.Board.ClearHighlights();
                break;
            case UnitState.Attacking:
                ShowPossibleTargets?.Invoke(this, _tile);
                if(_combat == null || !_combat.isActiveAndEnabled)
                {
                    _state = UnitState.Waiting;
                }
                break;
            default:
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 1);
        var gizmoSize = 0.8f;
        Gizmos.DrawLine(new Vector3(-gizmoSize / 2 + transform.position.x, 0, -gizmoSize / 2 + transform.position.z), new Vector3(-gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z));
        Gizmos.DrawLine(new Vector3(gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z), new Vector3(-gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z));
        Gizmos.DrawLine(new Vector3(gizmoSize / 2 + transform.position.x, 0, gizmoSize / 2 + transform.position.z), new Vector3(gizmoSize / 2 + transform.position.x, 0, -gizmoSize / 2 + transform.position.z));
        Gizmos.DrawLine(new Vector3(-gizmoSize / 2f + transform.position.x, 0, -gizmoSize / 2 + transform.position.z), new Vector3(gizmoSize / 2 + transform.position.x, 0, -gizmoSize / 2 + transform.position.z));
        var style = GUIStyle.none;
        style.fontSize = 32;
        style.alignment = TextAnchor.MiddleCenter;

        Handles.Label(transform.position + new Vector3(0, 1.5f, 0), _order.ToString(), style);
    }
}

public enum UnitState
{
    Moving,
    Attacking,
    Waiting
}