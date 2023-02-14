using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    private List<LevelSO> _levels;
    [SerializeField] private LevelTile _levelTilePrefab;
    [SerializeField] private GameObject _content;

    private void Awake()
    {
        _levels = Resources.LoadAll<LevelSO>("Levels").ToList();
        LevelTile.PlayLevel += PlayLevel;
    }
    private void OnDisable()
    {
        LevelTile.PlayLevel -= PlayLevel;
    }

    private void Start()
    {

        for(var i = 0; i < _levels.Count;i++)
        {
            var levelTile = Instantiate(_levelTilePrefab, _content.transform);
            levelTile.Init(i + 1 + "\n Level", i);
            if (!_levels[i].IsLocked)
            {
                levelTile.Unlock();
            }
        }
    }
    public void PlayLevel(int level)
    {
        GameManager.Instance.CurrentLevel = level;
        GameManager.Instance.UpdateGameState(GameState.GenerateBoard);
        Destroy(gameObject);
    }

    public void Back()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        Destroy(gameObject);
    }
}
