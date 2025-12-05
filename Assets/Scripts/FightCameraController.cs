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
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    public float startPositionX;
    public float characterDistance;

    public float EscalaDeMovimento = 0.5f;

    void Start()
    {
        startPositionX = transform.position.x;
    }

    void LateUpdate()
    {
        UpdateCamera();
    }


    /// <summary>
    /// Atualiza posição da câmera a cada frame - mantém foco no centro entre os personagens
    /// Ajusta distância dinamicamente baseado na distância entre os personagens
    /// </summary>
    public void UpdateCamera()
    {
        // Calcular distância entre personagens
        characterDistance = Vector3.Distance(player.position, enemy.position);
        if (characterDistance >= maxDistance) return;
        transform.position = new Vector3(
            startPositionX + (characterDistance),
            transform.position.y,
            transform.position.z
        );
    }
}
