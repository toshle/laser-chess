
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitBar : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Unit _unit;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image[] _healthOrbs;
    [SerializeField] private Image _background;

    [Header("Icon Settings")]
    [SerializeField] private Image _icon;
    [SerializeField] private Sprite _waitingIcon;
    [SerializeField] private Sprite _movingIcon;
    [SerializeField] private Sprite _attackingIcon;
    [SerializeField] private Sprite _deadIcon;

    [Header("Color settings")]
    [SerializeField] private Color _deadColor;
    [SerializeField] private Color _movingColor;
    [SerializeField] private Color _attackingColor;
    [SerializeField] private Color _waitingColor;

    [Header("Selection settings")]
    [SerializeField] private GameObject _hightlightFrame;

    void Update()
    {
        UpdateHealth(_unit.HP, _unit.MaxHP);
        if (!_unit.IsDead)
        {
            switch (_unit.State)
            {
                case UnitState.Moving:
                    _icon.sprite = _movingIcon;
                    _icon.color = _movingColor;
                    break;
                case UnitState.Attacking:
                    _icon.sprite = _attackingIcon;
                    _icon.color = _attackingColor;
                    break;
                case UnitState.Waiting:
                    _icon.sprite = _waitingIcon;
                    _icon.color = _waitingColor;
                    break;
                default: break;
            }
        }
        else
        {
            _background.color = _deadColor;
            _icon.sprite = _deadIcon;
            _icon.color = _deadColor;

        }

    }
    private void UpdateHealth(float current, float max)
    {
        for (int i = 0; i < _healthOrbs.Length; i++)
        {
            if (i >= current)
            {
                _healthOrbs[i].color = _deadColor;
            }
            if (i >= max)
            {
                _healthOrbs[i].gameObject.SetActive(false);
            }
        }
    }

    public void Init(Unit unit) {
        _unit = unit;
        _nameText.text = unit.Name;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hightlightFrame.SetActive(true);
        _unit.Tile.ShowHighlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hightlightFrame.SetActive(false);
        _unit.Tile.HideHighlight();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_unit.IsDead && GameManager.Instance.State == GameState.PlayerTurn)
        {
            _unit.Tile.Select();
            _hightlightFrame.SetActive(true);
        }
    }
}
