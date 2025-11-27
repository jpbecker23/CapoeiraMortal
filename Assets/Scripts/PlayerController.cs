using UnityEngine;
using System.Collections;

/// <summary>
/// Controlador do jogador - Gerencia movimento, combate e animações
/// Responsável por: movimento do jogador, ataques, esquivas e sincronização com Animator
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Animator animator;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private Camera mainCamera;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Vector3 moveDirection;
    private bool isAttacking = false;
    private bool isDodging = false;
    private float currentSpeed = 0f;

    void Start()
    {

    }

    void Update()
    {
        HandleMovement(); // Mover personagem
        HandleCombat(); // Processar combate
    }

    /// <summary>
    /// Processa movimento do jogador baseado no input
    /// Movimento é relativo à câmera (WASD move na direção da câmera)
    /// </summary>
    private void HandleMovement()
    {
        // Se está atacando ou fazendo esquiva, não pode se mover
        if (isAttacking || isDodging)
        {
            currentSpeed = 0f;
            return;
        }

        // Obter input do jogador (WASD ou setas)
        float horizontal = Input.GetAxis("Horizontal"); // A/D ou setas esquerda/direita
        moveDirection = new Vector3(0f, 0f, horizontal);

        // Se há movimento significativo (maior que 0.1)
        if (Mathf.Abs(horizontal) >= 0.1f)
        {
            transform.Translate(moveSpeed * Time.deltaTime * moveDirection);
            currentSpeed = moveSpeed; // Atualizar velocidade atual
        }
        else
        {
            currentSpeed = 0f; // Parado
        }
    }

    /// <summary>
    /// Processa input de combate - detecta teclas pressionadas e executa ataques
    /// Teclas: K=Bencao, J=Armada, L=Chapa, U=Rasteira, I=Couro, Space=Esquiva
    /// </summary>
    public void HandleCombat()
    {
        // Se já está atacando ou fazendo esquiva, não processar novo input
        if (isAttacking || isDodging) return;

        // Verificar cada tecla e executar ataque correspondente
        // Parâmetros: (nome do trigger, delay do ataque, duração da animação, multiplicador de dano)
        // if (Input.GetKeyDown(KeyCode.K)) PerformAttack("Bencao", 0.3f, 1.2f);
        // else if (Input.GetKeyDown(KeyCode.J)) PerformAttack("Armada", 0.4f, 1.5f);
        // else if (Input.GetKeyDown(KeyCode.L)) PerformAttack("Chapa", 0.3f, 1.0f);
        // else if (Input.GetKeyDown(KeyCode.U)) PerformAttack("Rasteira", 0.2f, 1.0f);
        // else if (Input.GetKeyDown(KeyCode.I)) PerformAttack("Couro", 0.5f, 2.0f, 1.5f); // Ataque especial com mais dano
        // else if (Input.GetKeyDown(KeyCode.Space)) PerformDodge(); // Esquiva
    }

    private void PerformAttack(string triggerName, float attackDelay, float damageMultiplier = 1f)
    {
        isAttacking = true;
        currentSpeed = 0f; // Parar movimento durante ataque

        if (combatSystem != null)
        {
            StartCoroutine(ExecuteAttackAfterDelay(attackDelay, damageMultiplier));
        }
    }

    private void PerformDodge()
    {
        isDodging = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDodgeSound();

        StartCoroutine(ResetDodgeStateAfterDelay(0.8f));
    }

    public void ExecuteAttack()
    {
        combatSystem.PlayerPerformAttack("Attack", 1f);
        AudioManager.Instance.PlayAttackSound();
    }

    private void ResetAttackState()
    {
        isAttacking = false;
        if (animator != null && animator.enabled)
        {
            try
            {
                animator.SetBool("IsAttacking", false);
            }
            catch { }
        }
    }

    private void ResetDodgeState()
    {
        isDodging = false;
    }

    private IEnumerator ExecuteAttackAfterDelay(float delay, float multiplier)
    {
        yield return new WaitForSeconds(delay);
        if (multiplier > 1.4f)
            ExecuteAttack();
    }

    private IEnumerator ResetAttackStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetAttackState();
    }

    private IEnumerator ResetDodgeStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetDodgeState();
    }
}
