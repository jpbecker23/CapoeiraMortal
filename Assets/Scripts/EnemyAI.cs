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
    [SerializeField] private EnemyHealthBar enemyHealthBar;
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
        
    }
    
    /// <summary>
    /// Loop principal da IA - decide ação baseado na distância do jogador
    /// Estados: Atacar (perto), Perseguir (médio), Idle (longe)
    /// </summary>
    void Update()
    {
        
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
        UpdateAnimator(); // Atualizar animações
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
        
        if (agent != null && agent.isOnNavMesh && agent.enabled)
        {
            agent.isStopped = false; // Permitir movimento
            agent.SetDestination(playerTarget.position); // Definir destino
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
