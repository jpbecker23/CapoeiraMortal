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
    [SerializeField] private float distance = 15f;
    [SerializeField] private float height = 5f;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 20f;
    
    private Vector3 targetPosition;
    private Vector3 centerPoint;
    
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        if (mainCamera == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCamera = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";
        }
        
        // Buscar personagens
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        
        if (enemy == null)
        {
            GameObject enemyObj = GameObject.FindGameObjectWithTag("Enemy");
            if (enemyObj != null) enemy = enemyObj.transform;
        }
        
        SetupCamera();
    }
    
    void LateUpdate()
    {
        if (player == null || enemy == null) return;
        
        UpdateCameraPosition();
    }
    
    void SetupCamera()
    {
        if (mainCamera == null) return;
        
        // Configurar câmera para combate
        mainCamera.orthographic = false;
        mainCamera.fieldOfView = 60f;
        mainCamera.nearClipPlane = 0.3f;
        mainCamera.farClipPlane = 100f;
        
        // Posição inicial
        if (player != null && enemy != null)
        {
            centerPoint = (player.position + enemy.position) / 2f;
            targetPosition = centerPoint + Vector3.back * distance + Vector3.up * height;
            mainCamera.transform.position = targetPosition;
            mainCamera.transform.LookAt(centerPoint);
        }
    }
    
    /// <summary>
    /// Atualiza posição da câmera a cada frame - mantém foco no centro entre os personagens
    /// Ajusta distância dinamicamente baseado na distância entre os personagens
    /// </summary>
    void UpdateCameraPosition()
    {
        // Calcular ponto central entre os personagens (meio do caminho)
        centerPoint = (player.position + enemy.position) / 2f;
        
        // Calcular distância entre personagens
        float characterDistance = Vector3.Distance(player.position, enemy.position);
        
        // Ajustar distância da câmera baseado na distância dos personagens
        // Se personagens estão longe, câmera fica mais longe também
        float dynamicDistance = Mathf.Clamp(characterDistance * 1.5f, minDistance, maxDistance);
        
        // Calcular posição alvo da câmera (atrás do centro, elevada)
        targetPosition = centerPoint + Vector3.back * dynamicDistance + Vector3.up * height;
        
        // Mover câmera suavemente até a posição alvo (interpolação linear)
        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position, 
            targetPosition, 
            followSpeed * Time.deltaTime
        );
        
        // Fazer câmera olhar para o centro entre os personagens
        Vector3 lookTarget = centerPoint;
        lookTarget.y = (player.position.y + enemy.position.y) / 2f; // Altura média
        mainCamera.transform.LookAt(lookTarget);
    }
    
    public void SetTargets(Transform playerTransform, Transform enemyTransform)
    {
        player = playerTransform;
        enemy = enemyTransform;
    }
}
