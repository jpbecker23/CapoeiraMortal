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
    [SerializeField] private float moveSpeed = 3f;

    [Header("Nível")]
    [SerializeField] private int currentLevel = 1;

    private bool isAttacking = false;

    private int[] attackTypes = new int[2];
    void Start()
    {

    }
    void Update()
    {

        float distance = Vector3.Distance(transform.position, playerTarget.position);
        if (isAttacking) return;
        else if (distance <= attackRange)
        {
            Attack();
            isAttacking = false;
        }
        else if (distance <= detectionRange)
        {
            Chase(); 
        }
    }

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
        int attackType = attackTypes[Random.Range(0, attackTypes.Length)];
        if (attackType == 0)
        {
            animator.SetTrigger("Pontera");
        }
        else if (attackType == 1)
        {
            animator.SetTrigger("ChuteAlto");
        }
        else if (attackType == 2)
        {
            animator.SetTrigger("Esquiva");
        }
    }

    private void ExecuteEnemyAttack()
    {
        float multiplier = 1f + (currentLevel - 1) * 0.3f;
        combatSystem.EnemyPerfomAttack("EnemyAttack", multiplier);
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
