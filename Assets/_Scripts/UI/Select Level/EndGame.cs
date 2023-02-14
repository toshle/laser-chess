using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Faction _winner;
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private GameObject _nextLevelButton;

    public void Init(Faction winner)
    {
        _winner = winner;
        if(winner == Faction.Human)
        {
            _winText.text = "Humans win";
        } else
        {
            _winText.text = "AI wins";
            _nextLevelButton.gameObject.SetActive(false);
        }
        if (GameManager.Instance.CurrentLevel == GameManager.Instance.LevelsCount - 1)
        {
            _nextLevelButton.gameObject.SetActive(false);
        }
    }

    public void MainMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        Destroy(gameObject);
    }

    public void NextLevel()
    {
        if(GameManager.Instance.CurrentLevel < GameManager.Instance.LevelsCount - 1)
        {
            GameManager.Instance.CurrentLevel += 1;
            GameManager.Instance.UpdateGameState(GameState.GenerateBoard);
            Destroy(gameObject);
        }
    }

}
