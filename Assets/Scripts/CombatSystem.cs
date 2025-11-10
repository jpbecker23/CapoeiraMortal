using UnityEngine;

/// <summary>
/// Sistema de combate - Detecta colisões de ataques e causa dano
/// Responsável por: detectar inimigos no alcance, calcular dano e aplicar em HealthSystem
/// </summary>
public class CombatSystem : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    
    [Header("Cooldown")]
    [SerializeField] private float attackCooldown = 0.5f;
    
    private float lastAttackTime = 0f;
    private HealthSystem healthSystem;
    
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown;
    public float AttackDamage => attackDamage;
    
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        
        // Se attackPoint não foi atribuído, usar transform
        if (attackPoint == null)
        {
            // Tentar encontrar AttackPoint como child
            Transform attackPointTransform = transform.Find("AttackPoint");
            if (attackPointTransform != null)
            {
                attackPoint = attackPointTransform;
            }
            else
            {
                // Criar AttackPoint
                GameObject attackPointObj = new GameObject("AttackPoint");
                attackPointObj.transform.SetParent(transform);
                attackPointObj.transform.localPosition = new Vector3(0, 1, 1); // Na frente
                attackPoint = attackPointObj.transform;
            }
        }
        
        // Se enemyLayer não foi configurado, tentar configurar automaticamente
        if (enemyLayer.value == 0)
        {
            // Determinar layer baseado na tag
            if (gameObject.CompareTag("Player"))
            {
                int enemyLayerIndex = LayerMask.NameToLayer("Enemy");
                if (enemyLayerIndex != -1)
                {
                    enemyLayer = 1 << enemyLayerIndex;
                }
            }
            else if (gameObject.CompareTag("Enemy"))
            {
                int playerLayerIndex = LayerMask.NameToLayer("Player");
                if (playerLayerIndex != -1)
                {
                    enemyLayer = 1 << playerLayerIndex;
                }
            }
        }
    }
    
    /// <summary>
    /// Executa um ataque - detecta inimigos no alcance e causa dano
    /// Usa OverlapSphere para detectar colisões em área esférica
    /// </summary>
    /// <param name="attackName">Nome do ataque (para debug)</param>
    /// <param name="damageMultiplier">Multiplicador de dano (para ataques especiais)</param>
    /// <returns>True se acertou algo, False se não acertou</returns>
    public bool PerformAttack(string attackName, float damageMultiplier = 1f)
    {
        // Verificar se pode atacar (cooldown)
        if (!CanAttack) return false;
        
        lastAttackTime = Time.time; // Registrar tempo do ataque
        
        // Calcular posição do ataque (usar AttackPoint se existir, senão usar posição à frente)
        Vector3 attackPosition = attackPoint != null ? attackPoint.position : transform.position + transform.forward * 1f;
        
        // Detectar inimigos no alcance usando esfera de colisão
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, attackRange, enemyLayer);
        
        bool hitSomething = false;
        
        // Processar cada inimigo acertado
        foreach (Collider enemy in hitEnemies)
        {
            HealthSystem enemyHealth = enemy.GetComponent<HealthSystem>();
            // Verificar se tem HealthSystem e não é o próprio personagem
            if (enemyHealth != null && enemyHealth != healthSystem)
            {
                float finalDamage = attackDamage * damageMultiplier; // Calcular dano final
                enemyHealth.TakeDamage(finalDamage); // Aplicar dano
                hitSomething = true;
                
                // Tocar som de hit
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayHitSound();
                
                #if UNITY_EDITOR
                Debug.Log($"{gameObject.name} causou {finalDamage} de dano em {enemy.name}");
                #endif
            }
        }
        
        // Se enemyLayer não está funcionando, tentar por tag (fallback)
        if (!hitSomething && enemyLayer.value == 0)
        {
            // Buscar todos os colliders no alcance
            Collider[] allColliders = Physics.OverlapSphere(attackPosition, attackRange);
            foreach (Collider col in allColliders)
            {
                bool isEnemy = false;
                // Verificar se é inimigo baseado na tag
                if (gameObject.CompareTag("Player") && col.CompareTag("Enemy"))
                    isEnemy = true;
                else if (gameObject.CompareTag("Enemy") && col.CompareTag("Player"))
                    isEnemy = true;
                
                if (isEnemy)
                {
                    HealthSystem enemyHealth = col.GetComponent<HealthSystem>();
                    if (enemyHealth != null && enemyHealth != healthSystem)
                    {
                        float finalDamage = attackDamage * damageMultiplier;
                        enemyHealth.TakeDamage(finalDamage);
                        
                        if (AudioManager.Instance != null)
                            AudioManager.Instance.PlayHitSound();
                    }
                }
            }
        }
        
        return hitSomething; // Retornar se acertou algo
    }
    
    void OnDrawGizmosSelected()
    {
        Vector3 attackPosition = attackPoint != null ? attackPoint.position : transform.position + transform.forward * 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
}
