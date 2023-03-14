using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] private Vector2 _position;

    [SerializeField] private Board _board;

    [SerializeField] private Unit _unit;

    [SerializeField] private EffectBase _effect;

    [SerializeField] private TileState _state;

    private bool _isSelected = false;

    [Header("Renderers")]
    [SerializeField] private SpriteRenderer _baseRenderer;
    [SerializeField] private SpriteRenderer _borderRenderer;
    [SerializeField] private SpriteRenderer _highlightRenderer;
    [SerializeField] private SpriteRenderer _iconRenderer;

    [Header("Icons")]
    [SerializeField] private Sprite _moveIcon;
    [SerializeField] private Sprite _targetIcon;
    [SerializeField] private Sprite _waitingIcon;

    [Header("Colors")]
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _offsetColor;
    [SerializeField] private Color _defaultGridColor;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private Color _movementHighlightColor;
    [SerializeField] private Color _targetHighlightColor;
    [SerializeField] private Color _waitingHighlightColor;

    public static event Action<Tile> TileSelected;
    public static event Action<Tile, bool> TileHovered;

    private Color _tileColor;

    public Board Board => _board;

    public Vector2 Position { 
        get { return _position; }
        set { _position = value; }
    }
    public Unit Unit
    {
        get { return _unit; }
        set { _unit = value; }
    }

    public EffectBase Effect
    {
        get { return _effect; }
        set { _effect = value; }
    }

    public bool IsSelected
    {
        get { return _isSelected; }
        set { 
            _isSelected = value;
            if (_isSelected)
            {
                SetHighlight(TileState.Highlighted);
            } else
            {
                SetHighlight(TileState.Default);
            }
        }
    }

    private void OnMouseUp()
    {
        Select();
    }

    private void OnMouseEnter()
    {
        TileHovered?.Invoke(this, true);
        ShowHighlight();
    }

    private void OnMouseExit()
    {
        TileHovered?.Invoke(this, false);
        HideHighlight();
    }

    public void Init(int x, int y, Board board)
    {
        var isOffset = (x + y) % 2 == 1;
        _tileColor = isOffset ? _offsetColor : _baseColor;
        _position = new Vector3(x, y);
        _board = board;
        _baseRenderer.color = _tileColor;
    }

    public void SetHighlight(TileState state) {
        _state = state;
        switch (state)
        {
            case TileState.Highlighted:
                _borderRenderer.color = _highlightColor;
                _highlightRenderer.color = _highlightColor;
                _highlightRenderer.gameObject.SetActive(true);
                break;
            case TileState.Move:
                _iconRenderer.gameObject.SetActive(true);
                _iconRenderer.sprite = _moveIcon;
                _iconRenderer.color = _movementHighlightColor;
                _borderRenderer.color = _movementHighlightColor;
                _highlightRenderer.color = _movementHighlightColor;
                break;
            case TileState.Target:
                _iconRenderer.gameObject.SetActive(true);
                _iconRenderer.sprite = _targetIcon;
                _iconRenderer.color = _targetHighlightColor;
                _borderRenderer.color = _targetHighlightColor;
                _highlightRenderer.color = _targetHighlightColor;
                break;
            case TileState.Waiting:
                _iconRenderer.gameObject.SetActive(true);
                _iconRenderer.sprite = _waitingIcon;
                _iconRenderer.color = _waitingHighlightColor;
                _borderRenderer.color = _waitingHighlightColor;
                _highlightRenderer.color = _waitingHighlightColor;
                break;
            default:
                _borderRenderer.color = _defaultGridColor;
                _highlightRenderer.color = _defaultGridColor;
                _iconRenderer.gameObject.SetActive(false);
                break;
        }
    }

    public void ShowHighlight() {
        _highlightRenderer.gameObject.SetActive(true);
    }

    public void HideHighlight()
    {
        _highlightRenderer.gameObject.SetActive(false);
    }
    public void Select()
    {
        if(GameManager.Instance.State == GameState.PlayerTurn && !GameManager.Instance.IsPaused)
        {
            TileSelected?.Invoke(this);
        }
    }
}

public enum TileState { 
    Default,
    Highlighted,
    Move,
    Target,
    Waiting
}