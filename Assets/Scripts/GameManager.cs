using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gerenciador principal do jogo - Singleton que controla estado do jogo
/// Responsável por: gerenciar níveis, encontrar personagens, pausar jogo, gerenciar vida
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Níveis")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel = 5;
    
    [Header("Referências")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private EnemyAI enemyAI;
    
    private HealthSystem playerHealth;
    private HealthSystem enemyHealth;
    
    private bool isGamePaused = false;
    private bool isGameOver = false;
    
    public int CurrentLevel => currentLevel;
    public bool IsGamePaused => isGamePaused;
    public bool IsGameOver => isGameOver;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadGameProgress();
        StartCoroutine(InitializeAfterFrame());
    }
    
    private IEnumerator InitializeAfterFrame()
    {
        yield return null;
        yield return null;
        
        FindAndActivateCharacters();
        InitializeHealthSystems();
        
        // Garantir visibilidade
        if (player != null)
        {
            player.SetActive(true);
            foreach (Renderer r in player.GetComponentsInChildren<Renderer>())
                if (r != null) r.enabled = true;
        }
        
        if (enemy != null)
        {
            enemy.SetActive(true);
            foreach (Renderer r in enemy.GetComponentsInChildren<Renderer>())
                if (r != null) r.enabled = true;
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReinitializeAfterSceneLoad());
    }
    
    private IEnumerator ReinitializeAfterSceneLoad()
    {
        yield return null;
        FindAndActivateCharacters();
        InitializeHealthSystems();
    }
    
    private void FindAndActivateCharacters()
    {
        // Player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
                player.SetActive(true);
                EnableComponents(player);
            }
        }
        else
        {
            if (!player.activeInHierarchy) player.SetActive(true);
            EnableComponents(player);
        }
        
        // Enemy
        if (enemy == null)
        {
            GameObject enemyObj = GameObject.FindGameObjectWithTag("Enemy");
            if (enemyObj != null)
            {
                enemy = enemyObj;
                enemy.SetActive(true);
                enemyAI = enemyObj.GetComponent<EnemyAI>();
                EnableComponents(enemy);
            }
        }
        else
        {
            if (!enemy.activeInHierarchy) enemy.SetActive(true);
            if (enemyAI == null) enemyAI = enemy.GetComponent<EnemyAI>();
            EnableComponents(enemy);
        }
    }
    
    /// <summary>
    /// Habilita todos os componentes necessários de um personagem
    /// Também configura NavMeshAgent procurando posição válida no NavMesh
    /// </summary>
    /// <param name="obj">GameObject do personagem (Player ou Enemy)</param>
    private void EnableComponents(GameObject obj)
    {
        if (obj == null) return;
        
        // Habilitar PlayerController se existir
        PlayerController pc = obj.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = true;
        
        // Habilitar EnemyAI se existir
        EnemyAI ai = obj.GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = true;
        
        // Habilitar Animator se existir
        Animator anim = obj.GetComponent<Animator>();
        if (anim != null) anim.enabled = true;
        
        // Habilitar CharacterController se existir
        CharacterController cc = obj.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;
        
        // Configurar NavMeshAgent se existir
        UnityEngine.AI.NavMeshAgent nav = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null)
        {
            // Verificar se posição atual está no NavMesh
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(obj.transform.position, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                // Se está no NavMesh, habilitar e mover agente para posição válida
                nav.enabled = true;
                nav.Warp(hit.position);
            }
            else
            {
                // Se não está no NavMesh, buscar posição válida próxima
                UnityEngine.AI.NavMeshHit hit2;
                Vector3 searchPos = obj.transform.position;
                bool found = false;
                
                // Buscar em círculos concêntricos (raio 1 até 10 unidades)
                for (float radius = 1f; radius <= 10f && !found; radius += 1f)
                {
                    // Buscar em 8 direções (45 graus cada)
                    for (int angle = 0; angle < 360; angle += 45)
                    {
                        float rad = angle * Mathf.Deg2Rad;
                        Vector3 testPos = searchPos + new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);
                        
                        // Verificar se esta posição está no NavMesh
                        if (UnityEngine.AI.NavMesh.SamplePosition(testPos, out hit2, 2f, UnityEngine.AI.NavMesh.AllAreas))
                        {
                            nav.enabled = true;
                            nav.Warp(hit2.position); // Mover agente
                            obj.transform.position = hit2.position; // Mover objeto também
                            found = true;
                            break;
                        }
                    }
                }
                
                // Se não encontrou NavMesh, desabilitar agente (usará movimento manual)
                if (!found)
                {
                    Debug.LogWarning($"NavMeshAgent em {obj.name} não conseguiu encontrar NavMesh válido. Desabilitando temporariamente.");
                    nav.enabled = false;
                }
            }
        }
    }
    
    private void InitializeHealthSystems()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath.RemoveListener(OnPlayerDeath);
                playerHealth.OnDeath.AddListener(OnPlayerDeath);
            }
        }
        
        if (enemy != null)
        {
            enemyHealth = enemy.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath.RemoveListener(OnEnemyDeath);
                enemyHealth.OnDeath.AddListener(OnEnemyDeath);
            }
        }
    }
    
    public void RefreshCharacterReferences()
    {
        FindAndActivateCharacters();
        InitializeHealthSystems();
    }
    
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, maxLevel);
        
        if (enemyAI != null)
            enemyAI.SetLevel(currentLevel);
        
        if (playerHealth != null)
            playerHealth.ResetHealth();
        
        if (enemyHealth != null)
            enemyHealth.ResetHealth();
        
        isGameOver = false;
        SaveSystem.SaveLevel(currentLevel);
    }
    
    public void NextLevel()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            SetLevel(currentLevel);
            if (UIManager.Instance != null)
                UIManager.Instance.ShowLevelComplete();
        }
        else
        {
            isGameOver = true;
            if (UIManager.Instance != null)
                UIManager.Instance.ShowLevelComplete();
        }
    }
    
    public void RestartLevel()
    {
        SetLevel(currentLevel);
        if (UIManager.Instance != null)
            UIManager.Instance.HideGameOver();
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        currentLevel = 1;
        StartCoroutine(RestartGameCoroutine());
    }
    
    private IEnumerator RestartGameCoroutine()
    {
        yield return null;
        FindAndActivateCharacters();
        InitializeHealthSystems();
        yield return null;
        SetLevel(currentLevel);
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOver();
            UIManager.Instance.HideMenu();
        }
    }
    
    private void OnPlayerDeath()
    {
        isGameOver = true;
        SaveSystem.AddLoss();
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver(false);
    }
    
    private void OnEnemyDeath()
    {
        isGameOver = true;
        SaveSystem.AddWin();
        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver(true);
        StartCoroutine(NextLevelAfterDelay(2f));
    }
    
    private void LoadGameProgress()
    {
        int savedLevel = SaveSystem.LoadLevel();
        if (savedLevel > 1 && savedLevel <= maxLevel)
            currentLevel = savedLevel;
    }
    
    private IEnumerator NextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextLevel();
    }
    
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
