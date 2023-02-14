using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private List<LevelSO> _levels;
    [SerializeField] private int _currentLevel;

    [SerializeField] private GameObject _canvasesContainer;
    [SerializeField] private MainMenu _mainMenuPrefab;
    [SerializeField] private PauseMenu _pauseMenuPrefab;
    [SerializeField] private SelectLevel _selectLevelPrefab;
    [SerializeField] private Board _boardPrefab;
    [SerializeField] private HUD _hudPrefab;
    [SerializeField] private EndGame _endGamePrefab;


    [SerializeField] private AIManager _AIManagerPrefab;

    private AIManager _AIManagerInstance;
    private Board _boardInstance;
    private HUD _hudInstance;

    private bool _isPaused;

    public bool IsPaused { 
        get { return _isPaused; }
        set { _isPaused = value; }
    }

    [SerializeField] public GameState State { get; private set; }
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }
    public int LevelsCount => _levels.Count;

    public static event Action TurnStarted;

    void Awake()
    {
        Instance = this;

        _levels = Resources.LoadAll<LevelSO>("Levels").ToList();
    }
    void Start()
    {
        _currentLevel = 0;
        UpdateGameState(GameState.MainMenu);
        HUD.PlayerTurnEnded += OnPlayerTurnEnded;
        AIManager.AITurnEnded += OnAITurnEnded;
        HUD.AITurnSkipped += OnAITurnSkipped;
        Combat.UnitDied += OnUnitDied;
    }

    private void OnDisable()
    {
        HUD.PlayerTurnEnded -= OnPlayerTurnEnded;
        AIManager.AITurnEnded -= OnAITurnEnded;
        HUD.AITurnSkipped -= OnAITurnSkipped;
        Combat.UnitDied -= OnUnitDied;
    }

    private void Update()
    {
        if (!_isPaused && Input.GetKeyDown(KeyCode.Escape) && (GameManager.Instance.State == GameState.PlayerTurn))
        {
            TogglePaused();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                if(_boardInstance != null)
                {
                    DestroyLevel();
                }
                SoundManager.Instance.PlayMenuMusic();
                Instantiate(_mainMenuPrefab, _canvasesContainer.transform);
                break;
            case GameState.SelectLevel:
                Instantiate(_selectLevelPrefab, _canvasesContainer.transform);
                break;
            case GameState.GenerateBoard:
                _boardInstance = Instantiate(_boardPrefab);
                _boardInstance.Init(_levels[_currentLevel]);
                _hudInstance = Instantiate(_hudPrefab, _canvasesContainer.transform);
                _hudInstance.Init(_boardInstance);
                _AIManagerInstance = Instantiate(_AIManagerPrefab);
                _AIManagerInstance.Init(_boardInstance);
                UpdateGameState(GameState.PlayerTurn);
                SoundManager.Instance.PlayBattleMusic();
                break;
            case GameState.PlayerTurn:
                TurnStarted?.Invoke();
                Debug.Log("Player turn");
                break;
            case GameState.AITurn:
                Debug.Log("AI turn");
                _AIManagerInstance.PlayTurn();
                break;
            case GameState.Win:
                DestroyLevel();
                var win = Instantiate(_endGamePrefab, _canvasesContainer.transform);
                win.Init(Faction.Human);
                break;
            case GameState.Lose:
                DestroyLevel();
                var lose = Instantiate(_endGamePrefab, _canvasesContainer.transform);
                lose.Init(Faction.AI);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

    }
    private void OnUnitDied(Unit unit)
    {
        if(unit.Faction == Faction.Human)
        {
            var unitsAlive = _boardInstance.Units.Where(unit => unit.Faction == Faction.Human && !unit.IsDead).Count();
            if(unitsAlive == 0)
            {
                UpdateGameState(GameState.Lose);
            }
        } else
        {
            if(unit.AI.Priority == 3)
            {
                var commandUnitsAlive = _boardInstance.Units.Where(unit => unit.Faction == Faction.AI && unit.AI.Priority == 3 && !unit.IsDead).Count();
                if (commandUnitsAlive == 0)
                {
                    UpdateGameState(GameState.Win);
                }
            }
        }
    }
    private void OnPlayerTurnEnded() 
    {
        UpdateGameState(GameState.AITurn);
    }
    private void OnAITurnEnded(bool win)
    {
        if(win)
        {
            UpdateGameState(GameState.Lose);
        } else
        {
            UpdateGameState(GameState.PlayerTurn);
        }
    }
    private void OnAITurnSkipped()
    {
        _AIManagerInstance.ActionDelay = 100;
    }

    public void TogglePaused()
    {
        if(State == GameState.PlayerTurn)
        {
            if(_isPaused)
            {
                _boardInstance.gameObject.SetActive(true);
                _hudInstance.gameObject.SetActive(true);
                _isPaused = false;
            } else
            {
                _boardInstance.gameObject.SetActive(false);
                _hudInstance.gameObject.SetActive(false);
                Instantiate(_pauseMenuPrefab, _canvasesContainer.transform);
                _isPaused = true;
            }
        }
    }

    private void DestroyLevel()
    {
        Destroy(_AIManagerInstance.gameObject);
        Destroy(_boardInstance.gameObject);
        Destroy(_hudInstance.gameObject);
    }
}

public enum GameState
{
    MainMenu,
    SelectLevel,
    GenerateBoard,
    PlayerTurn,
    AITurn,
    Win,
    Lose
}