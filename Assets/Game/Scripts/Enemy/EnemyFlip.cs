using UnityEngine;

/// <summary>
/// Voltea el sprite del enemigo según la dirección en que se mueve.
/// Lee la dirección desde EnemyPatrol sin depender de su lógica interna.
/// Requiere: EnemyPatrol en el mismo GameObject
/// </summary>
[RequireComponent(typeof(EnemyPatrol))]
public class EnemyFlip : MonoBehaviour
{
    [Header("Flip Settings")]

    [Tooltip("Si está activo, el sprite mira a la derecha por defecto en el proyecto." +
             "Desactívalo si tu sprite original mira a la izquierda")]
    [SerializeField] private bool spriteDefaultFacingRight = true;

    // Dirección que tenía el frame anterior para detectar cambios
    private float _lastDirectionX;

    private EnemyPatrol _patrol;

    private void Awake()
    {
        _patrol = GetComponent<EnemyPatrol>();

        // Inicializa con la dirección actual para evitar un flip en el primer frame
        _lastDirectionX = _patrol.GetPatrolDirection().x;
    }

    private void Update()
    {
        float currentDirectionX = _patrol.GetPatrolDirection().x;

        // Solo ejecuta el flip cuando la dirección cambia, no en cada frame
        if (Mathf.Approximately(currentDirectionX, _lastDirectionX)) return;

        // Invierte el scale X para voltear el sprite
        // Se usa localScale y no SpriteRenderer.flipX para que los hijos también se volteen
        Vector3 scale = transform.localScale;
        scale.x = spriteDefaultFacingRight ? currentDirectionX : -currentDirectionX;
        transform.localScale = scale;

        _lastDirectionX = currentDirectionX;
    }
}