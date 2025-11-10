using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Configura automaticamente os personagens na cena estilo Mortal Kombat
/// Responsável por: posicionar personagens, adicionar componentes necessários, configurar câmera
/// </summary>
public class CharacterSetup : MonoBehaviour
{
    [Header("Posicionamento")]
    [SerializeField] private Vector3 playerStartPosition = new Vector3(-2.5f, 0, 0);
    [SerializeField] private Vector3 enemyStartPosition = new Vector3(2.5f, 0, 0);
    
    [Header("Referências")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    
    void Start()
    {
        SetupCharacters();
    }
    
    /// <summary>
    /// Configura personagens na cena - encontra ou cria Player e Enemy
    /// Adiciona componentes necessários e posiciona corretamente
    /// </summary>
    [ContextMenu("Configurar Personagens")]
    public void SetupCharacters()
    {
        // Encontrar ou criar Player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null && playerPrefab != null)
        {
            // Se não existe, criar a partir do prefab
            player = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            player.tag = "Player";
            player.name = "Player";
        }
        
        if (player != null)
        {
            SetupPlayer(player); // Configurar componentes do Player
        }
        
        // Encontrar ou criar Enemy
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy == null && enemyPrefab != null)
        {
            // Se não existe, criar a partir do prefab
            enemy = Instantiate(enemyPrefab, enemyStartPosition, Quaternion.identity);
            enemy.tag = "Enemy";
            enemy.name = "Enemy";
        }
        
        if (enemy != null)
        {
            SetupEnemy(enemy); // Configurar componentes do Enemy
        }
        
        // Configurar câmera para estilo Mortal Kombat
        SetupCamera();
        
        Debug.Log("Personagens configurados!");
    }
    
    /// <summary>
    /// Configura componentes do Player - adiciona scripts necessários e posiciona
    /// </summary>
    /// <param name="player">GameObject do Player</param>
    void SetupPlayer(GameObject player)
    {
        // Posicionar Player na posição inicial (esquerda)
        player.transform.position = playerStartPosition;
        player.transform.rotation = Quaternion.LookRotation(Vector3.right); // Olhar para direita (em direção ao inimigo)
        
        // Garantir que tem PlayerController (movimento e combate)
        if (player.GetComponent<PlayerController>() == null)
            player.AddComponent<PlayerController>();
            
        // Garantir CharacterController (movimento físico)
        if (player.GetComponent<CharacterController>() == null)
        {
            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 2f; // Altura do personagem
            cc.radius = 0.5f; // Raio do collider
            cc.center = new Vector3(0, 1, 0); // Centro do collider
        }
        
        // Garantir Animator (animações)
        if (player.GetComponent<Animator>() == null)
            player.AddComponent<Animator>();
        else
        {
            Animator anim = player.GetComponent<Animator>();
            anim.enabled = true; // Habilitar Animator
            // Controller será atribuído via script Editor ou manualmente
        }
        
        // Garantir CombatSystem (sistema de combate)
        if (player.GetComponent<CombatSystem>() == null)
            player.AddComponent<CombatSystem>();
            
        // Garantir HealthSystem (sistema de vida)
        if (player.GetComponent<HealthSystem>() == null)
        {
            HealthSystem health = player.AddComponent<HealthSystem>();
            // Configurar via reflection ou deixar padrão
        }
        
        // Garantir que está ativo
        player.SetActive(true);
        
        // Habilitar todos os renderers (para garantir que está visível)
        foreach (Renderer r in player.GetComponentsInChildren<Renderer>())
            if (r != null) r.enabled = true;
    }
    
    /// <summary>
    /// Configura componentes do Enemy - adiciona scripts necessários e posiciona
    /// </summary>
    /// <param name="enemy">GameObject do Enemy</param>
    void SetupEnemy(GameObject enemy)
    {
        // Posicionar Enemy na posição inicial (direita)
        enemy.transform.position = enemyStartPosition;
        enemy.transform.rotation = Quaternion.LookRotation(Vector3.left); // Olhar para esquerda (em direção ao player)
        
        // Garantir EnemyAI (IA do inimigo)
        if (enemy.GetComponent<EnemyAI>() == null)
            enemy.AddComponent<EnemyAI>();
            
        // Garantir NavMeshAgent (navegação)
        if (enemy.GetComponent<NavMeshAgent>() == null)
        {
            NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
            agent.height = 2f; // Altura do agente
            agent.radius = 0.5f; // Raio do agente
            agent.speed = 3f; // Velocidade de movimento
            agent.stoppingDistance = 2.5f; // Distância de parada (para atacar)
        }
        
        // Garantir Animator (animações)
        if (enemy.GetComponent<Animator>() == null)
            enemy.AddComponent<Animator>();
        else
        {
            Animator anim = enemy.GetComponent<Animator>();
            anim.enabled = true; // Habilitar Animator
            // Controller será atribuído manualmente no Unity ou via script Editor
        }
        
        // Garantir CombatSystem (sistema de combate)
        if (enemy.GetComponent<CombatSystem>() == null)
            enemy.AddComponent<CombatSystem>();
            
        // Garantir HealthSystem (sistema de vida)
        if (enemy.GetComponent<HealthSystem>() == null)
            enemy.AddComponent<HealthSystem>();
        
        // Garantir que está ativo
        enemy.SetActive(true);
        
        // Habilitar todos os renderers (para garantir que está visível)
        foreach (Renderer r in enemy.GetComponentsInChildren<Renderer>())
            if (r != null) r.enabled = true;
    }
    
    void SetupCamera()
    {
        // Encontrar ou criar câmera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";
        }
        
        // Adicionar FightCameraController
        if (mainCam.GetComponent<FightCameraController>() == null)
        {
            FightCameraController camController = mainCam.gameObject.AddComponent<FightCameraController>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (player != null && enemy != null)
                camController.SetTargets(player.transform, enemy.transform);
        }
    }
}
