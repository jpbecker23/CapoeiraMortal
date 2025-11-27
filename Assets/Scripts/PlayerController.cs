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
        HandleMovement();
        HandleCombat();
    }

    /// <summary>
    /// Processa movimento do jogador baseado no input
    /// Movimento é relativo à câmera (WASD move na direção da câmera)
    /// </summary>
    private void HandleMovement()
    {
        if (isAttacking || isDodging)
        {
            currentSpeed = 0f;
            return;
        }

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

    public void HandleCombat()
    {
        if (isAttacking || isDodging) return;
        // if (Input.GetKeyDown(KeyCode.K)) PerformAttack("Bencao", 0.15f, 10f);
        // else if (Input.GetKeyDown(KeyCode.J)) PerformAttack("Armada", 1.05f, 10f);
        // else if (Input.GetKeyDown(KeyCode.L)) PerformAttack("Chapa", 0.14f, 10f);
        // else if (Input.GetKeyDown(KeyCode.U)) PerformAttack("Rasteira", 0.13f, 10f);
        // else if (Input.GetKeyDown(KeyCode.I)) PerformAttack("Couro", 1.15f, 10f);
    }

    private void PerformAttack(string triggerName, float delay, float damageMultiplier = 1f)
    {
        isAttacking = true;
        currentSpeed = 0f; // Parar movimento durante ataque
        StartCoroutine(ExecuteAttackAfterDelay(delay, damageMultiplier));
    }

    private void PerformDodge()
    {
        isDodging = true;
        StartCoroutine(ResetDodgeStateAfterDelay(0.8f));
    }

    public void ExecuteAttack()
    {
        HandleCombat();
        //AudioManager.Instance.PlayAttackSound();
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
