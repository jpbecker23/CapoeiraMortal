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
    [SerializeField] private Camera mainCamera;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Vector3 moveDirection;
    private bool isAttacking = false;
    private bool isDodging = false;
    private float currentSpeed = 0f;
    public AudioSource PlayerAudio;
    public AudioClip attackSound;

    public GameObject enemyTarget;


    void Start()
    {
        PlayerAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleMovement();
        Chase();
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

    private void Chase()
    {
        // Calcular direção até o jogador
        Vector3 direction = (enemyTarget.transform.position - transform.position).normalized;
        direction.y = 0; // Ignorar altura (movimento apenas no plano XZ)

        // Rotacionar inimigo para olhar na direção do jogador
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
        }
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

    private IEnumerator ExecuteAttackAfterDelay(float delay, float multiplier)
    {
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator ResetAttackStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetAttackState();
    }
}
