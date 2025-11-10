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
    [SerializeField] private CharacterController controller;
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private Camera mainCamera;
    
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    
    private Vector3 moveDirection;
    private bool isAttacking = false;
    private bool isDodging = false;
    private float currentSpeed = 0f;
    
    /// <summary>
    /// Inicializa componentes e força Animator a funcionar corretamente
    /// </summary>
    void Start()
    {
        // Buscar componentes automaticamente se não foram atribuídos
        if (animator == null) animator = GetComponent<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        if (combatSystem == null) combatSystem = GetComponent<CombatSystem>();
        if (mainCamera == null) mainCamera = Camera.main;
        
        // Configurar Animator para garantir que funcione corretamente
        if (animator != null)
        {
            animator.enabled = true;
            animator.updateMode = AnimatorUpdateMode.Normal; // Atualizar sempre
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate; // Sempre animar
            
            // FORCAR que Animator funcione - garantir que nao esta em T-pose
            if (animator.runtimeAnimatorController != null)
            {
                animator.Rebind(); // Reiniciar bindings do Animator
                // Forcar Idle inicial para evitar T-pose
                if (animator.isActiveAndEnabled)
                {
                    animator.Play("Idle", 0, 0f);
                    animator.Update(0f); // Atualizar imediatamente
                }
            }
        }
    }
    
    /// <summary>
    /// Loop principal - processa input, movimento, combate e animações a cada frame
    /// </summary>
    void Update()
    {
        // Verificações de segurança
        if (!gameObject.activeInHierarchy) return; // Se objeto está desativado, não processar
        if (GameManager.Instance != null && GameManager.Instance.IsGamePaused) return; // Se jogo está pausado, não processar
        if (controller == null) return; // Se não tem CharacterController, não processar
            
        HandleInput(); // Processar entrada do jogador
        HandleMovement(); // Mover personagem
        HandleCombat(); // Processar combate
        UpdateAnimator(); // Atualizar parâmetros do Animator
    }
    
    void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        if (combatSystem == null) combatSystem = GetComponent<CombatSystem>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (animator != null)
        {
            animator.enabled = true;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            
            // FORCAR Rebind quando habilitado
            if (animator.runtimeAnimatorController != null)
            {
                animator.Rebind();
                if (animator.isActiveAndEnabled)
                {
                    animator.Play("Idle", 0, 0f);
                }
            }
        }
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
        float vertical = Input.GetAxis("Vertical"); // W/S ou setas cima/baixo
        
        // Calcular direção relativa à câmera (para movimento funcionar independente da rotação da câmera)
        Vector3 forward = mainCamera != null ? mainCamera.transform.forward : Vector3.forward; // Frente da câmera
        Vector3 right = mainCamera != null ? mainCamera.transform.right : Vector3.right; // Direita da câmera
        forward.y = 0f; // Ignorar componente Y (altura)
        right.y = 0f;
        forward.Normalize(); // Normalizar para ter tamanho 1
        right.Normalize();
        
        // Calcular direção final de movimento combinando horizontal e vertical
        moveDirection = (right * horizontal + forward * vertical).normalized;
        
        // Se há movimento significativo (maior que 0.1)
        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotacionar personagem para olhar na direção do movimento
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            // Mover usando CharacterController
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
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
    private void HandleCombat()
    {
        // Se já está atacando ou fazendo esquiva, não processar novo input
        if (isAttacking || isDodging) return;
        
        // Verificar cada tecla e executar ataque correspondente
        // Parâmetros: (nome do trigger, delay do ataque, duração da animação, multiplicador de dano)
        if (Input.GetKeyDown(KeyCode.K)) PerformAttack("Bencao", 0.3f, 1.2f);
        else if (Input.GetKeyDown(KeyCode.J)) PerformAttack("Armada", 0.4f, 1.5f);
        else if (Input.GetKeyDown(KeyCode.L)) PerformAttack("Chapa", 0.3f, 1.0f);
        else if (Input.GetKeyDown(KeyCode.U)) PerformAttack("Rasteira", 0.2f, 1.0f);
        else if (Input.GetKeyDown(KeyCode.I)) PerformAttack("Couro", 0.5f, 2.0f, 1.5f); // Ataque especial com mais dano
        else if (Input.GetKeyDown(KeyCode.Space)) PerformDodge(); // Esquiva
    }
    
    private void PerformAttack(string triggerName, float attackDelay, float duration, float damageMultiplier = 1f)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        if (!animator.enabled) animator.enabled = true;
        
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
        if (animator == null) return;
        if (!animator.enabled) animator.enabled = true;
        
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
        if (combatSystem != null)
            combatSystem.PerformAttack("Attack", 1f);
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAttackSound();
    }
    
    public void ExecuteAttackSpecial()
    {
        if (combatSystem != null)
            combatSystem.PerformAttack("Special", 1.5f);
        
        if (AudioManager.Instance != null)
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
    
    /// <summary>
    /// Atualiza parâmetros do Animator baseado no estado do jogador
    /// Sincroniza Speed e IsMoving com o movimento real
    /// Força animação Idle quando parado para evitar T-pose
    /// </summary>
    private void UpdateAnimator()
    {
        if (animator == null) return;
        
        // GARANTIR que Animator está habilitado e tem Controller
        if (!animator.enabled) 
        {
            animator.enabled = true;
            #if UNITY_EDITOR
            Debug.LogWarning("PlayerController: Animator estava desabilitado! Habilitado agora.");
            #endif
        }
        
        // Se não tem Controller atribuído, tentar carregar automaticamente
        if (animator.runtimeAnimatorController == null)
        {
            // Tentar encontrar Controller nos Resources
            RuntimeAnimatorController animController = Resources.Load<RuntimeAnimatorController>("ProtagonistaController");
            if (animController != null)
            {
                animator.runtimeAnimatorController = animController;
                animator.Rebind(); // Reiniciar bindings
                #if UNITY_EDITOR
                Debug.Log("PlayerController: Controller atribuído em runtime!");
                #endif
            }
            else
            {
                // Se não encontrar, não atualizar (evita erros)
                return;
            }
        }
        
        // Calcular velocidade atual do personagem
        float speed = 0f;
        if (controller != null && controller.enabled)
        {
            speed = controller.velocity.magnitude; // Velocidade do CharacterController
        }
        else
        {
            speed = currentSpeed; // Velocidade calculada manualmente
        }
        
        // Normalizar velocidade (0 a 1) para o Animator
        float normalizedSpeed = Mathf.Clamp01(speed / moveSpeed);
        
        // Atualizar parâmetros do Animator
        try
        {
            animator.SetFloat("Speed", normalizedSpeed); // Velocidade normalizada (0-1)
            animator.SetBool("IsMoving", speed > 0.1f); // Se está se movendo
            
            // FORCAR que não esteja em T-pose - se velocidade é 0, garantir Idle
            if (speed < 0.1f && !isAttacking && !isDodging)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                // Se não está em Idle e já passou um pouco da animação atual, forçar Idle
                if (!stateInfo.IsName("Idle") && stateInfo.normalizedTime > 0.1f)
                {
                    try
                    {
                        animator.Play("Idle", 0, 0f); // Forçar animação Idle
                    }
                    catch { }
                }
            }
        }
        catch (System.Exception e)
        {
            #if UNITY_EDITOR
            Debug.LogWarning($"Erro ao atualizar Animator: {e.Message}");
            #endif
        }
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
