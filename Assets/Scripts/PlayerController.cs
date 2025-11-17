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
   //[SerializeField] private CharacterController controller;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private Camera mainCamera;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Vector3 moveDirection;
    private bool isAttacking = false;
    private bool isDodging = false;
    private float currentSpeed = 0f;

    /// <summary>
    /// Inicializa componentes e força Animator a funcionar corretamente
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// Loop principal - processa input, movimento, combate e animações a cada frame
    /// </summary>
    void Update()
    {
        HandleInput(); // Processar entrada do jogador
        HandleMovement(); // Mover personagem
        HandleCombat(); // Processar combate
    }


    private void HandleInput()
    {
        // Input processado em HandleMovement e HandleCombat
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
        //float vertical = Input.GetAxis("Vertical"); // W/S ou setas cima/baixo

        // Calcular direção relativa à câmera (para movimento funcionar independente da rotação da câmera)
        Vector3 forward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward; // Frente da câmera
        Vector3 right = mainCamera != null ? mainCamera.transform.right : Vector3.right; // Direita da câmera
        forward.y = 0f; // Ignorar componente Y (altura)
        right.y = 0f;
        forward.Normalize(); // Normalizar para ter tamanho 1
        right.Normalize();

        // Calcular direção final de movimento combinando horizontal e vertical
        //moveDirection = (right * horizontal + forward * vertical).normalized;
        moveDirection = (right * horizontal).normalized;

        // Se há movimento significativo (maior que 0.1)
        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotacionar personagem para olhar na direção do movimento
            //Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            // Mover usando CharacterController
            //controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
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

    private void PerformAttack(string triggerName, float attackDelay, float duration, float damageMultiplier = 1f)
    {
        isAttacking = true;
        currentSpeed = 0f; // Parar movimento durante ataque

        try
        {
            animator.SetTrigger(triggerName);
            animator.SetBool("IsAttacking", true);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Erro ao executar trigger {triggerName}: {e.Message}");
        }

        if (combatSystem != null)
        {
            StartCoroutine(ExecuteAttackAfterDelay(attackDelay, damageMultiplier));
        }

        StartCoroutine(ResetAttackStateAfterDelay(duration));
    }

    private void PerformDodge()
    {
        isDodging = true;
        currentSpeed = 0f;

        try
        {
            animator.SetTrigger("Dodge");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Erro ao executar Dodge: {e.Message}");
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDodgeSound();

        StartCoroutine(ResetDodgeStateAfterDelay(0.8f));
    }

    public void ExecuteAttack()
    {
        combatSystem.PerformAttack("Attack", 1f);
        AudioManager.Instance.PlayAttackSound();
    }

    public void ExecuteAttackSpecial()
    {
        combatSystem.PerformAttack("Special", 1.5f);
        AudioManager.Instance.PlayAttackSound();
    }

    public void OnAttackHit() => ExecuteAttack();
    public void OnSpecialAttackHit() => ExecuteAttackSpecial();
    public void OnAttackEnd() => ResetAttackState();
    public void OnDodgeEnd() => ResetDodgeState();

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
            ExecuteAttackSpecial();
        else
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
