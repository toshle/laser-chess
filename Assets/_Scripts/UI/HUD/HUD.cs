using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [SerializeField] private GameObject _leftSideContainer;
    [SerializeField] private GameObject _rightSideContainer;
    [SerializeField] private GameObject _warningContainer;
    [SerializeField] private GameObject _endTurnButton;
    [SerializeField] private GameObject _skipAITurnButton;
    [SerializeField] private TextMeshProUGUI _currentTurnText;

    [SerializeField] private UnitBar _leftUnitPrefab;
    [SerializeField] private UnitBar _rightUnitPrefab;

    [SerializeField] private Board _board;

    private List<UnitBar> _unitBars = new();

    public static event Action PlayerTurnEnded;
    public static event Action AITurnSkipped;

    void Start()
    {
        Board.UnitSpawned += AddUnitBar;
        GameManager.TurnStarted += OnTurnStarted;
    }

    private void OnDisable()
    {
        GameManager.TurnStarted -= OnTurnStarted;
        Board.UnitSpawned -= AddUnitBar;
    }

    void Update()
    {
        _currentTurnText.text = GameManager.Instance.State.ToString();
    }

    private void OnTurnStarted()
    {
        _endTurnButton.gameObject.SetActive(true);
        _skipAITurnButton.gameObject.SetActive(false);
    }

    public void Init(Board board)
    {
        _board = board; 
        foreach (var unit in _board.Units)
        {
            AddUnitBar(unit);
        }
    }

    public void AddUnitBar(Unit unit)
    {
        if (unit.Faction == Faction.Human)
        {
            var unitBar = Instantiate(_leftUnitPrefab, _leftSideContainer.transform);
            unitBar.Init(unit);
            _unitBars.Add(unitBar);
        }
        else
        {
            var unitBar = Instantiate(_rightUnitPrefab, _rightSideContainer.transform);
            unitBar.Init(unit);
            _unitBars.Add(unitBar);
        }
        SortBars();
    }

    public void SortBars()
    {
        foreach(var bar in _unitBars) {
            bar.transform.SetSiblingIndex(bar.Unit.Order);
        }
    }

    public void ShowWarning() 
    {
        var hasUnmovedUnits = _board.Units.Where(unit => unit.Faction == Faction.Human && !unit.IsDead && unit.State != UnitState.Waiting).Any();
        if(hasUnmovedUnits)
        {
            _endTurnButton.gameObject.SetActive(false);
            _warningContainer.gameObject.SetActive(true);
        } else
        {
            EndTurn();
        }
    }

    public void HideWarning()
    {
        _warningContainer.gameObject.SetActive(false);
    }


    public void EndTurn()
    {
        _warningContainer.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(false);
        _skipAITurnButton.gameObject.SetActive(true);
        PlayerTurnEnded?.Invoke();
    }

    public void SkipAITurn()
    {
        _skipAITurnButton.gameObject.SetActive(false);
        AITurnSkipped?.Invoke();
    }
}
