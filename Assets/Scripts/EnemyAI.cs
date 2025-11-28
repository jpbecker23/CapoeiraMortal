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

    public float EnemyPower = 10f;

    [Header("Configurações")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Nível")]
    [SerializeField] private int currentLevel = 1;

    private bool isAttacking = false;

    private readonly int[] attackTypes = new int[3];

    void Start()
    {

    }
    public void Update()
    {

        float distance = Vector3.Distance(transform.position, playerTarget.position);
        if (distance <= detectionRange)
        {
            Chase();
            float valorPadrao = Random.Range(0f, 5f);
            if (!isAttacking)
            {
                isAttacking = true;
                Invoke("Attack", valorPadrao);
            }
        }
        else
        {
            isAttacking = false;
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

    public void Attack()
    {

        isAttacking = true;

        int attackType = Random.Range(0, attackTypes.Length + 1);
        switch (attackType)
        {
            case 0:
                animator.SetTrigger("Pontera");
                combatSystem.EnemyPerfomAttack("Pontera", EnemyPower);
                break;
            case 1:
                animator.SetTrigger("ChuteAlto");
                combatSystem.EnemyPerfomAttack("ChuteAlto", EnemyPower);
                break;
            case 2:
                animator.SetTrigger("Esquiva");
                combatSystem.EnemyPerfomAttack("Esquiva", 0f);
                break;
            default:
                animator.SetTrigger("Pontera");
                combatSystem.EnemyPerfomAttack("Pontera", EnemyPower);
                break;
        }
        isAttacking = false;
    }

    public void AttackDelay(float delay)
    {
        StartCoroutine(ExecuteAttackAfterDelay(5.0f));
    }

    private void ResetAttack()
    {
        StartCoroutine(ExecuteAttackAfterDelay(5.0f));
    }

    public IEnumerator ExecuteAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // isAttacking = false;
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
        animator.SetTrigger("Death");

        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}