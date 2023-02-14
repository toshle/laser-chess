using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (GameManager.Instance.State == GameState.PlayerTurn))
        {
            Resume();
        }
    }
    public void Resume()
    {
        Time.timeScale = 1;
        GameManager.Instance.TogglePaused();
        Destroy(gameObject);
    }

    public void MainMenu()
    {
        GameManager.Instance.IsPaused = false;
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        Destroy(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
