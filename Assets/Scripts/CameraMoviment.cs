using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Configurações")]
    public float delay = 1.2f;               // Suavidade da câmera
    public float yOffset = -0.19f;           // Ajuste vertical da câmera
    public float distMultiplier = 0.45f;     // O quanto a distância do Player afeta o zoom

    [Header("Referências")]
    public Transform enemy;                  // Enemy
    public Transform player;                 // Player 

    public Transform point;                  // Ponto central mapa

    private Vector3 targetPos;

    void LateUpdate()
    {
        // Distância apenas no eixo X entre player e ponto central
        float zDist = Mathf.Abs(player.position.z - enemy.position.z);

        // Calcula quanto "zoom" aplicar conforme a distância
        float zoomValue = zDist * distMultiplier;

        // A posição final da câmera (ela só se mexe no eixo X)
        float targetX = enemy.position.x + zoomValue;

        // Mantém Y da câmera fixo com offset
        float targetY = transform.position.y + yOffset;

        // Mantém Z da câmera
        float targetZ = (player.position.z + enemy.position.z) / 2;

        // if (player.position.z < -6 || player.position.z > 6)
        // {
        //     targetZ = (player.position.z + enemy.position.z) / 2;
        // }
        // else
        // {
        //     targetZ = point.position.z;
        // }

        // Posição desejada
        targetPos = new Vector3(targetX, targetY, targetZ);

        // Movimento suave
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * delay
        );
    }
}
