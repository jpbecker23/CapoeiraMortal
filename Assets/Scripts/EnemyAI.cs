using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// IA do inimigo - Controla comportamento do inimigo (perseguição, ataque, animações)
/// Responsável por: detectar jogador, perseguir, atacar e gerenciar NavMeshAgent
/// </summary>
public class EnemyAI : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Transform playerTarget;
    
    [Header("Configurações")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float moveSpeed = 3f;
    
    [Header("Nível")]
    [SerializeField] private int currentLevel = 1;
    
    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    
    private string[] attackTypes = { "ChuteAlto", "Pontera", "Ginga" };
    
    void Start()
    {
        // Buscar componentes
        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (combatSystem == null) combatSystem = GetComponent<CombatSystem>();
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
        
        // Buscar player
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
        
        // Configurar NavMesh
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = attackRange - 0.5f;
            SetupNavMesh();
        }
        
        if (animator != null) animator.enabled = true;
        if (healthSystem != null) healthSystem.OnDeath.AddListener(OnDeath);
    }
    
    /// <summary>
    /// Loop principal da IA - decide ação baseado na distância do jogador
    /// Estados: Atacar (perto), Perseguir (médio), Idle (longe)
    /// </summary>
    void Update()
    {
        // Verificações de segurança
        if (!gameObject.activeInHierarchy) return; // Se desativado, não processar
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused) return; // Se jogo pausado, não processar
        if (playerTarget == null) return; // Se não tem alvo (jogador), não processar
        
        // Calcular distância até o jogador
        float distance = Vector3.Distance(transform.position, playerTarget.position);
        
        // Se está atacando, não fazer nada mais
        if (isAttacking) return;
        
        // Decidir ação baseado na distância
        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack(); // Muito perto e pode atacar -> Atacar
        }
        else if (distance <= detectionRange)
        {
            Chase(); // Dentro do alcance de detecção -> Perseguir
        }
        else
        {
            Idle(); // Muito longe -> Ficar parado
        }
        
        UpdateAnimator(); // Atualizar animações
    }
    
    void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
        if (animator != null) animator.enabled = true;
        if (agent != null) SetupNavMesh();
    }
    
    /// <summary>
    /// Configura NavMeshAgent - tenta encontrar posição válida no NavMesh
    /// Busca em círculos concêntricos até encontrar NavMesh válido
    /// Se não encontrar, desabilita NavMeshAgent (inimigo usará movimento manual)
    /// </summary>
    private void SetupNavMesh()
    {
        if (agent == null) return;
        
        NavMeshHit hit;
        // Tentar posição atual primeiro
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // Mover agente para posição válida
            agent.enabled = true;
        }
        else
        {
            // Se posição atual não está no NavMesh, buscar em múltiplas direções
            bool found = false;
            Vector3 searchPos = transform.position;
            
            // Buscar em círculos concêntricos (raio 1 até 15 unidades)
            for (float radius = 1f; radius <= 15f && !found; radius += 1f)
            {
                // Buscar em 12 direções (30 graus cada)
                for (int angle = 0; angle < 360; angle += 30)
                {
                    float rad = angle * Mathf.Deg2Rad;
                    Vector3 testPos = searchPos + new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);
                    
                    // Verificar se esta posição está no NavMesh
                    if (NavMesh.SamplePosition(testPos, out hit, 3f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position); // Mover agente
                        transform.position = hit.position; // Mover objeto também
                        agent.enabled = true;
                        found = true;
                        Debug.Log($"NavMesh encontrado em posição alternativa: {hit.position}");
                        break;
                    }
                }
            }
            
            // Se não encontrou NavMesh em lugar nenhum, desabilitar agente
            if (!found)
            {
                Debug.LogWarning($"NavMesh não encontrado próximo a {gameObject.name}. Desabilitando NavMeshAgent temporariamente.");
                agent.enabled = false; // Inimigo usará movimento manual (fallback)
            }
        }
    }
    
    private void Idle()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
        
        // Forcar animacao Idle se existir, senao usar Ginga
        if (animator != null && animator.enabled)
        {
            try
            {
                // Tentar Idle primeiro
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!stateInfo.IsName("Idle"))
                {
                    animator.Play("Idle", 0, 0f);
                }
            }
            catch
            {
                // Se Idle nao existe, usar Ginga (que e o default)
                try
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (!stateInfo.IsName("Ginga"))
                    {
                        animator.Play("Ginga", 0, 0f);
                    }
                }
                catch { }
            }
        }
    }
    
    /// <summary>
    /// Persegue o jogador - rotaciona e move em direção ao alvo
    /// Usa NavMeshAgent se disponível, senão usa movimento manual direto
    /// </summary>
    private void Chase()
    {
        // Calcular direção até o jogador
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        direction.y = 0; // Ignorar altura (movimento apenas no plano XZ)
        
        // Rotacionar inimigo para olhar na direção do jogador
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
        }
        
        // Movimento com NavMesh se disponível (melhor - evita obstáculos)
        if (agent != null && agent.isOnNavMesh && agent.enabled)
        {
            agent.isStopped = false; // Permitir movimento
            agent.SetDestination(playerTarget.position); // Definir destino
        }
        else
        {
            // Fallback: movimento manual sem NavMesh (movimento direto, pode atravessar obstáculos)
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    
    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
        
        // Escolher ataque
        string attackType = attackTypes[Random.Range(0, attackTypes.Length)];
        if (currentLevel >= 2 && Random.value < 0.3f)
            attackType = "Esquiva";
        
        // Animar
        if (animator != null)
        {
            if (!animator.enabled) animator.enabled = true;
            animator.Play(attackType, 0, 0f);
            animator.SetBool("IsAttacking", true);
        }
        
        // Causar dano
        if (combatSystem != null)
        {
            float delay = attackType == "Ginga" ? 0.5f : 0.3f;
            StartCoroutine(ExecuteAttackAfterDelay(delay));
        }
        
        // Resetar
        float duration = attackType == "Ginga" ? 1.5f : 1.0f;
        StartCoroutine(ResetAttackAfterDelay(duration));
    }
    
    private void ExecuteEnemyAttack()
    {
        if (combatSystem != null)
        {
            float multiplier = 1f + (currentLevel - 1) * 0.3f;
            combatSystem.PerformAttack("EnemyAttack", multiplier);
            }
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAttackSound();
        }
    
    public void OnEnemyAttackHit() => ExecuteEnemyAttack();
    public void OnEnemyAttackEnd() => ResetAttack();
    
    private void ResetAttack()
    {
        isAttacking = false;
        if (animator != null && animator.enabled)
            animator.SetBool("IsAttacking", false);
    }
    
    private void UpdateAnimator()
    {
        if (animator == null) return;
        if (!animator.enabled) animator.enabled = true;
        
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsMoving", speed > 0.1f);
    }
    
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 5);
        if (agent != null)
        {
            float multiplier = 1f + (currentLevel - 1) * 0.2f;
            agent.speed = moveSpeed * multiplier;
        }
    }
    
    private void OnDeath()
    {
        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;
        
        if (animator != null && animator.enabled)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == "Death" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    animator.SetTrigger("Death");
                    break;
                }
            }
        }
        
        enabled = false;
    }
    
    private IEnumerator ExecuteAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ExecuteEnemyAttack();
    }
    
    private IEnumerator ResetAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetAttack();
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
