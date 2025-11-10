using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Gerenciador de interface do usuário - Controla menu, HUD, barras de vida e pausa
/// Responsável por: exibir menus, atualizar barras de vida, mostrar game over, gerenciar pausa
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Menu Principal")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    
    [Header("HUD do Jogo")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private Slider enemyHealthBar;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    [SerializeField] private Image playerHealthFill;
    [SerializeField] private Image enemyHealthFill;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    
    [Header("Tela de Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button nextLevelButton;
    
    [Header("Tela de Vitória de Nível")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private Button continueButton;
    
    private HealthSystem playerHealth;
    private HealthSystem enemyHealth;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeUI();
        SetupButtons();
        FindHealthSystems();
    }
    
    private void InitializeUI()
    {
        // Mostrar menu inicialmente
        if (menuPanel != null)
            menuPanel.SetActive(true);
        
        // Esconder HUD e Game Over
        if (hudPanel != null)
            hudPanel.SetActive(false);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        // Configurar sliders de vida para não serem interativos e esconder handles
        ConfigureHealthBars();
    }
    
    private void ConfigureHealthBars()
    {
        // Configurar PlayerHealthBar
        if (playerHealthBar != null)
        {
            // Desabilitar interatividade (não pode ser arrastado)
            playerHealthBar.interactable = false;
            
            // Esconder elementos desnecessários (handles, background)
            HideSliderUnnecessaryElements(playerHealthBar.transform);
        }
        
        // Configurar EnemyHealthBar
        if (enemyHealthBar != null)
        {
            // Desabilitar interatividade (não pode ser arrastado)
            enemyHealthBar.interactable = false;
            
            // Esconder elementos desnecessários (handles, background)
            HideSliderUnnecessaryElements(enemyHealthBar.transform);
        }
    }
    
    private void HideSliderUnnecessaryElements(Transform sliderTransform)
    {
        if (sliderTransform == null) return;
        
        // Lista de elementos que devem ser escondidos
        string[] elementsToHide = { "Handle Slide Area", "Handle", "Background" };
        
        // Esconder cada elemento da lista
        foreach (string elementName in elementsToHide)
        {
            Transform element = sliderTransform.Find(elementName);
            if (element != null)
            {
                // Desabilitar o GameObject
                element.gameObject.SetActive(false);
                
                // Desabilitar o componente Image se existir (garantir que não renderize)
                Image img = element.GetComponent<Image>();
                if (img != null)
                {
                    img.enabled = false;
                }
            }
        }
        
        // Procurar recursivamente por elementos que devem ser escondidos
        foreach (Transform child in sliderTransform)
        {
            // Esconder Handle Slide Area ou qualquer coisa com "Handle"
            if (child.name.Contains("Handle"))
            {
                child.gameObject.SetActive(false);
                Image img = child.GetComponent<Image>();
                if (img != null) img.enabled = false;
            }
            // Esconder Background (barra branca de fundo)
            else if (child.name == "Background")
            {
                child.gameObject.SetActive(false);
                Image img = child.GetComponent<Image>();
                if (img != null) img.enabled = false;
            }
        }
        
        // Desabilitar qualquer componente Image no próprio Slider que possa estar causando a barra branca
        Image sliderImage = sliderTransform.GetComponent<Image>();
        if (sliderImage != null)
        {
            // Se a imagem do slider estiver visível e branca, torná-la transparente
            if (sliderImage.enabled && sliderImage.color.a > 0.1f)
            {
                // Verificar se é uma cor branca ou muito clara
                if (sliderImage.color.r > 0.9f && sliderImage.color.g > 0.9f && sliderImage.color.b > 0.9f)
                {
                    sliderImage.enabled = false; // Desabilitar completamente
                }
            }
        }
        
        // Garantir que apenas o Fill Area fique visível (a barra colorida)
        Transform fillArea = sliderTransform.Find("Fill Area");
        if (fillArea != null)
        {
            fillArea.gameObject.SetActive(true);
        }
    }
    
    private void SetupButtons()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
        
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(NextLevel);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueToNextLevel);
        
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
    }
    
    private void FindHealthSystems()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged.AddListener(UpdatePlayerHealth);
            }
        }
        
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            enemyHealth = enemy.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.OnHealthChanged.AddListener(UpdateEnemyHealth);
            }
        }
    }
    
    void Update()
    {
        // Atualizar texto de nível
        if (levelText != null && GameManager.Instance != null)
        {
            int currentLevel = GameManager.Instance.CurrentLevel;
            levelText.text = $"Nível: {currentLevel}/5";
            
            // Adicionar indicador de dificuldade
            string difficulty = GetDifficultyText(currentLevel);
            if (difficulty != "")
            {
                levelText.text += $" - {difficulty}";
            }
        }
        
        // Verificar pausa (ESC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Atualizar texto de controles se necessário
        UpdateControlsText();
    }
    
    void LateUpdate()
    {
        // Garantir que os elementos desnecessários dos sliders permaneçam escondidos
        // Isso previne que sejam reativados por algum motivo
        if (hudPanel != null && hudPanel.activeInHierarchy)
        {
            if (playerHealthBar != null && playerHealthBar.gameObject.activeInHierarchy)
            {
                HideSliderUnnecessaryElements(playerHealthBar.transform);
            }
            if (enemyHealthBar != null && enemyHealthBar.gameObject.activeInHierarchy)
            {
                HideSliderUnnecessaryElements(enemyHealthBar.transform);
            }
        }
    }
    
    private string GetDifficultyText(int level)
    {
        switch (level)
        {
            case 1: return "Fácil";
            case 2: return "Normal";
            case 3: return "Difícil";
            case 4: return "Muito Difícil";
            case 5: return "Extremo";
            default: return "";
        }
    }
    
    private void UpdateControlsText()
    {
        if (controlsText != null)
        {
            // Texto de controles mais legível
            controlsText.text = "CONTROLES:\n" +
                               "WASD - Movimento | " +
                               "K-Bencao | J-Armada | L-Chapa\n" +
                               "U-Rasteira | I-Couro | Espaço-Esquiva | ESC-Pausa";
        }
    }
    
    public void StartGame()
    {
        // Garantir que o jogo não está pausado
        Time.timeScale = 1f;
        
        // Tocar som de UI
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUISound(0);
        }
        
        if (menuPanel != null)
            menuPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(true);
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
        
        // Garantir que os handles dos sliders estejam escondidos quando o HUD aparecer
        ConfigureHealthBars();
        
        if (GameManager.Instance != null)
        {
            // Forçar reencontrar personagens antes de reiniciar
            GameManager.Instance.RefreshCharacterReferences();
            
            // Aguardar um frame antes de reiniciar o jogo
            StartCoroutine(StartGameCoroutine());
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogError("UIManager: GameManager.Instance é null! Verifique se GameManager está na cena.");
            #endif
        }
    }
    
    private IEnumerator StartGameCoroutine()
    {
        yield return null; // Aguardar um frame
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        
        // Reencontrar sistemas de vida após iniciar o jogo
        yield return new WaitForSeconds(0.2f);
        FindHealthSystems();
    }
    
    
    public void HideMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(true);
        
        // Garantir que os handles dos sliders estejam escondidos
        ConfigureHealthBars();
    }
    
    public void ShowGameOver(bool playerWon)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
            {
                gameOverText.text = playerWon ? "Você Venceu!" : "Você Perdeu!";
                gameOverText.color = playerWon ? Color.green : Color.red;
            }
            
            // Mostrar botão de próximo nível apenas se venceu
            if (nextLevelButton != null)
                nextLevelButton.gameObject.SetActive(playerWon);
        }
        
        if (hudPanel != null)
            hudPanel.SetActive(false);
    }
    
    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (hudPanel != null)
            hudPanel.SetActive(true);
        
        // Garantir que os handles dos sliders estejam escondidos
        ConfigureHealthBars();
    }
    
    public void ShowLevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            
            if (levelCompleteText != null && GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentLevel >= 5)
                {
                    levelCompleteText.text = "Parabéns! Você completou todos os níveis!";
                }
                else
                {
                    levelCompleteText.text = $"Nível {GameManager.Instance.CurrentLevel} Completo!";
                }
            }
        }
    }
    
    public void HideLevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }
    
    private void UpdatePlayerHealth(float health)
    {
        if (playerHealthBar != null && playerHealth != null)
        {
            float percentage = playerHealth.HealthPercentage;
            playerHealthBar.value = percentage;
            
            // Atualizar texto de vida
            if (playerHealthText != null)
            {
                playerHealthText.text = $"Player: {Mathf.CeilToInt(playerHealth.CurrentHealth)}/{Mathf.CeilToInt(playerHealth.MaxHealth)}";
            }
            
            // Mudar cor da barra baseado na vida
            if (playerHealthFill != null)
            {
                UpdateHealthBarColor(playerHealthFill, percentage);
            }
        }
    }
    
    private void UpdateEnemyHealth(float health)
    {
        if (enemyHealthBar != null && enemyHealth != null)
        {
            float percentage = enemyHealth.HealthPercentage;
            enemyHealthBar.value = percentage;
            
            // Atualizar texto de vida
            if (enemyHealthText != null)
            {
                enemyHealthText.text = $"Inimigo: {Mathf.CeilToInt(enemyHealth.CurrentHealth)}/{Mathf.CeilToInt(enemyHealth.MaxHealth)}";
            }
            
            // Mudar cor da barra baseado na vida
            if (enemyHealthFill != null)
            {
                UpdateHealthBarColor(enemyHealthFill, percentage, true);
            }
        }
    }
    
    private void UpdateHealthBarColor(Image fillImage, float percentage, bool isEnemy = false)
    {
        if (fillImage == null) return;
        
        Color targetColor;
        
        if (isEnemy)
        {
            // Enemy: Vermelho → Laranja → Vermelho Escuro
            if (percentage > 0.5f)
                targetColor = Color.red;
            else if (percentage > 0.25f)
                targetColor = new Color(1f, 0.5f, 0f); // Laranja
            else
                targetColor = new Color(0.7f, 0f, 0f); // Vermelho Escuro
        }
        else
        {
            // Player: Verde → Amarelo → Vermelho
            if (percentage > 0.5f)
                targetColor = Color.green;
            else if (percentage > 0.25f)
                targetColor = Color.yellow;
            else
                targetColor = Color.red;
        }
        
        fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * 5f);
    }
    
    public void TogglePause()
    {
        if (GameManager.Instance == null) return;
        
        if (GameManager.Instance.IsGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    
    public void PauseGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }
    
    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
        
        // Garantir que os handles dos sliders estejam escondidos
        ConfigureHealthBars();
    }
    
    private void RestartLevel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    }
    
    private void NextLevel()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextLevel();
        }
        HideGameOver();
    }
    
    private void ContinueToNextLevel()
    {
        HideLevelComplete();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NextLevel();
        }
    }
    
    private void ReturnToMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
        
        if (menuPanel != null)
            menuPanel.SetActive(true);
        
        if (hudPanel != null)
            hudPanel.SetActive(false);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    private void QuitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

