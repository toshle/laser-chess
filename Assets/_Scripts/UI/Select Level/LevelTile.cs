using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int _level = 0;
    [SerializeField] private Image _tile;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _lockedText;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private Color _lockedColor;
    [SerializeField] private Color _lockedLabelColor;
    [SerializeField] private Color _lockedTileColor;
    [SerializeField] private Color _unlockedTileColor;
    [SerializeField] private Color _unlockedColor;

    [SerializeField] private bool _isLocked = true;
    [SerializeField] private string _name;

    public static event Action<int> PlayLevel;

    public void Init(string name, int level)
    {
        _tile.color = _lockedTileColor;
        _text.color = _lockedColor;
        _lockedText.color = _lockedLabelColor;
        _lockedText.gameObject.SetActive(true);
        _name = name;
        _text.text = name;
        _level = level;
    }

    public void Unlock()
    {
        _lockedText.gameObject.SetActive(false);
        _tile.color = _unlockedTileColor;
        _text.color = _unlockedColor;
        _isLocked = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            PlayLevel?.Invoke(_level);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            _tile.color = _highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            _tile.color = _unlockedTileColor;
        }
    }
}
