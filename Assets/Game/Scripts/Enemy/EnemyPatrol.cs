using UnityEngine;

/// <summary>
/// Mueve al enemigo de derecha a izquierda dentro de una distancia fija
/// desde su posición inicial. No requiere configuración externa.
/// Requiere: Rigidbody2D (Freeze Rotation Z activado)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]

    [Tooltip("Distancia máxima que recorre hacia cada lado desde su posición inicial")]
    [SerializeField] private float patrolDistance = 3f;

    [Tooltip("Velocidad de movimiento horizontal")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Si está activo, comienza moviéndose hacia la derecha")]
    [SerializeField] private bool startMovingRight = true;

    // Posición X donde el enemigo apareció al inicio
    private Vector2 _startPosition;

    // 1 = derecha, -1 = izquierda
    private int _direction;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
        _direction = startMovingRight ? 1 : -1;
    }

    private void FixedUpdate()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        // Al llegar al límite derecho, gira a la izquierda
        if (distanceFromStart >= patrolDistance)
            _direction = -1;

        // Al llegar al límite izquierdo, gira a la derecha
        else if (distanceFromStart <= -patrolDistance)
            _direction = 1;

        // Aplica velocidad horizontal manteniendo la velocidad vertical (gravedad)
        _rb.linearVelocity = new Vector2(_direction * moveSpeed, _rb.linearVelocity.y);
    }

    /// <summary>
    /// Devuelve la dirección actual del enemigo como vector.
    /// Útil para que EnemyFlip u otros componentes lean la dirección sin acoplarse.
    /// </summary>
    public Vector2 GetPatrolDirection() => new Vector2(_direction, 0);
}