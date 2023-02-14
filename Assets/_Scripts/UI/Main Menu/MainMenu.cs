using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        GameManager.Instance.UpdateGameState(GameState.GenerateBoard);
        Destroy(gameObject);
    }

    public void SelectLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.SelectLevel);
        Destroy(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
