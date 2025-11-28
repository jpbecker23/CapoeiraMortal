using UnityEngine;

/// <summary>
/// Controlador de câmera estilo Mortal Kombat - foca nos dois personagens
/// Responsável por: posicionar câmera entre os personagens, ajustar distância dinamicamente
/// </summary>
public class FightCameraController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [SerializeField] private Camera mainCamera;

    [Header("Configurações")]
    [SerializeField] private float distance;
    [SerializeField] private float height;
    [SerializeField] private float followSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    private Vector3 targetPosition;
    private Vector3 centerPoint;
    public float EscalaDeMovimento = 0.5f;
    public float OffsetZBase = -10f;

    void Start()
    {
    }

    void LateUpdate()
    {
        if (player == null || enemy == null) return;

        UpdateCamera();
    }


    /// <summary>
    /// Atualiza posição da câmera a cada frame - mantém foco no centro entre os personagens
    /// Ajusta distância dinamicamente baseado na distância entre os personagens
    /// </summary>
    public void UpdateCamera()
    {
        // Calcular distância entre personagens
        float characterDistance = Vector3.Distance(player.position, enemy.position);
        float posicaoAlvoZ = (characterDistance * EscalaDeMovimento) + OffsetZBase;
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y,
            posicaoAlvoZ
            );
    }

    public void SetTargets(Transform playerTransform, Transform enemyTransform)
    {
        player = playerTransform;
        enemy = enemyTransform;
    }
}
