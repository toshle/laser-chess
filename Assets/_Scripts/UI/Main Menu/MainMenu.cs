using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _instructions;

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
    public void HowToPlay()
    {
        _mainMenu.SetActive(false);
        _instructions.SetActive(true);
    }

    public void Back()
    {
        _mainMenu.SetActive(true);
        _instructions.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
