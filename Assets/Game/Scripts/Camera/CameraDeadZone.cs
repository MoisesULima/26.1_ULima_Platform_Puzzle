using UnityEngine;

public class CameraDeadZone : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed  = 5f;

    // Tamaño de la dead zone (en unidades del mundo)
    [SerializeField] float deadZoneX = 1.5f;
    [SerializeField] float deadZoneY = 0.8f;

    void LateUpdate()
    {
        // Diferencia entre jugador y cámara (solo X,Y)
        float dx = target.position.x - transform.position.x;
        float dy = target.position.y - transform.position.y;

        // Destino: la cámara solo se mueve si el jugador
        // supera el límite de la dead zone
        float targetX = transform.position.x;
        float targetY = transform.position.y;

        if (Mathf.Abs(dx) > deadZoneX)
            targetX = target.position.x
                      - Mathf.Sign(dx) * deadZoneX;

        if (Mathf.Abs(dy) > deadZoneY)
            targetY = target.position.y
                      - Mathf.Sign(dy) * deadZoneY;

        // Lerp suave hacia el destino
        Vector3 dest = new Vector3(
            targetX, targetY, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position, dest,
            smoothSpeed * Time.deltaTime);
    }
}
