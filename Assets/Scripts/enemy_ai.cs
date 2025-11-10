using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Animator enemyAnim;
    private Transform player;
    
    [Header("Movement Settings")]
    public float detectionRange = 10f;      // Distância para detectar o player
    public float attackRange = 2.5f;        // Distância para atacar
    public float moveSpeed = 2f;            // Velocidade de movimento
    public float rotationSpeed = 5f;        // Velocidade de rotação
    
    [Header("Combat Settings")]
    public float attackCooldown = 3f;       // Tempo entre ataques
    private float lastAttackTime = 0f;
    
    [Header("State")]
    public bool isAttacking = false;
    private bool playerInRange = false;
    
    // Lista de ataques disponíveis (baseado no Animator Controller)
    private enum AttackType
    {
        ChuteAlto,
        Pontera,
        Esquiva
    }
    
    void Start()
    {
        // Pega o Animator do inimigo
        enemyAnim = GetComponent<Animator>();
        
        if (enemyAnim == null)
        {
            Debug.LogError("Animator não encontrado no inimigo! Adicione um Animator component.");
        }
        
        // Encontra o player pela tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player não encontrado! Certifique-se que o player tem a tag 'Player'");
        }
    }
    
    void Update()
    {
        if (player == null || enemyAnim == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Verifica se o player está no alcance de detecção
        playerInRange = distanceToPlayer <= detectionRange;
        
        if (playerInRange)
        {
            // Rotaciona em direção ao player
            RotateTowardsPlayer();
            
            // Se está longe, move em direção ao player
            if (distanceToPlayer > attackRange && !isAttacking)
            {
                MoveTowardsPlayer();
            }
            // Se está perto, para e ataca
            else if (distanceToPlayer <= attackRange)
            {
                StopMoving();
                TryAttack();
            }
        }
        else
        {
            // Player fora do alcance, para de se mover
            StopMoving();
        }
    }
    
    private void MoveTowardsPlayer()
    {
        // Move em direção ao player
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    
    private void StopMoving()
    {
        // Para o movimento (a animação Ginga já roda automaticamente)
    }
    
    private void RotateTowardsPlayer()
    {
        // Calcula direção para o player (apenas no plano horizontal)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Mantém rotação apenas no eixo Y
        
        if (direction != Vector3.zero)
        {
            // Cria rotação olhando para o player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            
            // Rotaciona suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    
    private void TryAttack()
    {
        // Verifica se pode atacar (cooldown)
        if (Time.time - lastAttackTime >= attackCooldown && !isAttacking)
        {
            PerformRandomAttack();
            lastAttackTime = Time.time;
        }
    }
    
    private void PerformRandomAttack()
    {
        // Escolhe um ataque aleatório
        AttackType randomAttack = (AttackType)Random.Range(0, System.Enum.GetValues(typeof(AttackType)).Length);
        
        isAttacking = true;
        enemyAnim.SetBool("IsAttacking", true);
        
        // Define a duração do ataque baseado no tipo
        float attackDuration = 0f;
        
        switch (randomAttack)
        {
            case AttackType.ChuteAlto:
                enemyAnim.Play("ChuteAlto");
                attackDuration = 1.5f;
                break;
            case AttackType.Pontera:
                enemyAnim.Play("Pontera");
                attackDuration = 1.3f;
                break;
            case AttackType.Esquiva:
                enemyAnim.Play("Esquiva");
                attackDuration = 1.2f;
                break;
        }
        
        // Reseta o estado após a animação terminar
        StartCoroutine(ResetAttackState(attackDuration));
    }
    
    private IEnumerator ResetAttackState(float delay)
    {
        yield return new WaitForSeconds(delay);
        isAttacking = false;
        enemyAnim.SetBool("IsAttacking", false);
        
        // Volta para Ginga (estado idle)
        enemyAnim.Play("Ginga");
    }
    
    // Visualização no Editor (esferas azul e vermelha para ver os ranges)
    private void OnDrawGizmosSelected()
    {
        // Mostra o alcance de detecção (azul)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Mostra o alcance de ataque (vermelho)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}