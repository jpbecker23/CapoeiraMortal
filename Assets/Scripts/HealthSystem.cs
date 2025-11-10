using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sistema de vida - Gerencia vida, morte e eventos relacionados
/// Responsável por: receber dano, curar, verificar morte e disparar eventos
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Eventos")]
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnDeath;
    
    private bool isDead = false;
    
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;
    public bool IsDead => isDead;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Aplica dano ao personagem
    /// Reduz vida atual e verifica se morreu
    /// </summary>
    /// <param name="damage">Quantidade de dano a receber</param>
    public void TakeDamage(float damage)
    {
        if (isDead) return; // Se já está morto, não receber mais dano
        
        currentHealth = Mathf.Max(0, currentHealth - damage); // Reduzir vida (mínimo 0)
        OnHealthChanged?.Invoke(currentHealth); // Notificar mudança de vida
        
        // Se vida chegou a zero, morrer
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Cura o personagem
    /// Aumenta vida atual até o máximo
    /// </summary>
    /// <param name="amount">Quantidade de vida a recuperar</param>
    public void Heal(float amount)
    {
        if (isDead) return; // Se está morto, não pode curar
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount); // Aumentar vida (máximo maxHealth)
        OnHealthChanged?.Invoke(currentHealth); // Notificar mudança de vida
    }
    
    /// <summary>
    /// Reseta vida para o máximo
    /// Usado ao reiniciar nível ou respawnar
    /// </summary>
    public void ResetHealth()
    {
        isDead = false; // Não está mais morto
        currentHealth = maxHealth; // Restaurar vida máxima
        OnHealthChanged?.Invoke(currentHealth); // Notificar mudança
    }
    
    /// <summary>
    /// Mata o personagem
    /// Toca som de morte e dispara evento OnDeath
    /// </summary>
    private void Die()
    {
        if (isDead) return; // Se já está morto, não fazer nada
        
        isDead = true;
        
        // Tocar som de morte
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDeathSound();
        }
        
        OnDeath?.Invoke(); // Disparar evento de morte (GameManager escuta isso)
    }
}

