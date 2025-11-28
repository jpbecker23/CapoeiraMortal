using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GerenciadorDeCena : MonoBehaviour
{
    public GameObject PauseUI;
    public GameObject NextLevelPanel;

    public void Update()
    {
        PauseGame();
    }

    public void CarregarCena()
    {
        SceneManager.LoadScene("Level1");
    }

    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        PauseUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void NextLevel()
    {
        NextLevelPanel.SetActive(true);
        //fazer logica de delay ou algo assim para primeiro carregar o sprite "YOUWIN" e depois o "NEXTLEVEL"
        Time.timeScale = 0f;
    }
}
