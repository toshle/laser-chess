using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private List<LevelSO> _levels;
    [SerializeField] private int _currentLevel;

    [SerializeField] private GameObject _canvasesContainer;
    [SerializeField] private GameObject _loadingPrefab;
    [SerializeField] private MainMenu _mainMenuPrefab;
    [SerializeField] private PauseMenu _pauseMenuPrefab;
    [SerializeField] private SelectLevel _selectLevelPrefab;
    [SerializeField] private Board _boardPrefab;
    [SerializeField] private HUD _hudPrefab;
    [SerializeField] private EndGame _endGamePrefab;


    [SerializeField] private AIManager _AIManagerPrefab;

    private AIManager _AIManagerInstance;
    public static Board BoardInstance;
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
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

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

    public async void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                if(BoardInstance != null)
                {
                    UnloadLevel();
                }
                SoundManager.Instance.PlayMenuMusic();
                Instantiate(_mainMenuPrefab, _canvasesContainer.transform);
                break;
            case GameState.SelectLevel:
                Instantiate(_selectLevelPrefab, _canvasesContainer.transform);
                break;
            case GameState.GenerateBoard:
                var loadingInstance = Instantiate(_loadingPrefab, _canvasesContainer.transform);
                await LoadLevel(_currentLevel);
                Destroy(loadingInstance);
                _hudInstance = Instantiate(_hudPrefab, _canvasesContainer.transform);
                _hudInstance.Init(BoardInstance);
                _AIManagerInstance = Instantiate(_AIManagerPrefab);
                _AIManagerInstance.Init(BoardInstance);
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
                UnloadLevel();
                var win = Instantiate(_endGamePrefab, _canvasesContainer.transform);
                win.Init(Faction.Human);
                break;
            case GameState.Lose:
                UnloadLevel();
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
            var unitsAlive = BoardInstance.Units.Where(unit => unit.Faction == Faction.Human && !unit.IsDead).Count();
            if(unitsAlive == 0)
            {
                UpdateGameState(GameState.Lose);
            }
        } else
        {
            if(unit.AI.Priority == 3)
            {
                var commandUnitsAlive = BoardInstance.Units.Where(unit => unit.Faction == Faction.AI && unit.AI.Priority == 3 && !unit.IsDead).Count();
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
                BoardInstance.gameObject.SetActive(true);
                _hudInstance.gameObject.SetActive(true);
                _isPaused = false;
            } else
            {
                BoardInstance.gameObject.SetActive(false);
                _hudInstance.gameObject.SetActive(false);
                Instantiate(_pauseMenuPrefab, _canvasesContainer.transform);
                _isPaused = true;
            }
        }
    }

    private async Task LoadLevel(int index)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_levels[index].sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            await Task.Yield();                
        }
    }

    private void UnloadLevel()
    {
        Destroy(_AIManagerInstance.gameObject);
        Destroy(BoardInstance.gameObject);
        Destroy(_hudInstance.gameObject);
        SceneManager.UnloadSceneAsync(_levels[_currentLevel].sceneName);
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