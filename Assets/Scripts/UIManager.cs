using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GerenciadorDeCena : MonoBehaviour
{
    public GameObject PauseUI;

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

    public void ExitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
