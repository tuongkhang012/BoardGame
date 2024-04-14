using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGameWithAI()
    {
        SceneManager.LoadScene("GameAI");
    }

    public void StartGameWithPlayer()
    {
        SceneManager.LoadScene("GameVS");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
