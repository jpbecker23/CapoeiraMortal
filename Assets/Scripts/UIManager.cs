using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CenasJogo
{
    Level1,
    Level2,
    Level3,
    Level4,
    level5,
    Load,
    MainMenu,
}

public class GerenciadorDeCena : MonoBehaviour
{
    public GameObject Panelcomandos;
    public GameObject MainPanel;
    public GameObject PauseUI;
    public GameObject NextLevelUI;
    public GameObject YouWinUI;

    public CenasJogo levelAtual;

    public CombatSystem combatSystem;
    private bool loadNextLevel = false;

    public void Start()
    {
        if (combatSystem != null)
        {
            combatSystem.nextLevelEvent.AddListener(NextLevelEventListener); // Add a listener
        }
    }

    public void NextLevelEventListener()
    {
        if ((int)levelAtual <= 5 && loadNextLevel)
        {
            NextLevelUI.SetActive(true);
            // YouWinUI.SetActive(false);
            int numCena = (int)levelAtual + 1;
            CenasJogo proximoNivel = (CenasJogo)numCena;
            Debug.Log(proximoNivel);
            //CarregarCena(proximoNivel);
            SceneManager.LoadScene(proximoNivel.ToString());
            return;
        } 
        else if(loadNextLevel) {
            YouWinUI.SetActive(true);
            SceneManager.LoadScene("MainMenu");
            return;
        }
        // YouWinUI.SetActive(true);
        Debug.Log("PRÓXIMO NÍVEL");
        Debug.Log(levelAtual);
        Invoke("NextLevelEventListener", 5f);


        loadNextLevel = true;
        return;
    }

    public void Update()
    {
        PauseGame();
    }

    public void CarregarCena(CenasJogo cenaName)
    {
        SceneManager.LoadScene(cenaName.ToString());
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Level1");
    }

    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            PauseUI.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PauseUI.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Panelcomandos.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void Comandos()
    {
        Panelcomandos.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
