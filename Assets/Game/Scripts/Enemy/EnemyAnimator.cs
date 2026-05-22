using UnityEngine;

/// <summary>
/// Controla los parámetros del Animator del enemigo según su estado actual.
/// Estados soportados: Idle, Walk, Attack, Death.
/// Requiere: Animator, EnemyPatrol en el mismo GameObject.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyPatrol))]
public class EnemyAnimator : MonoBehaviour
{
    // ─── Animator Parameter Names ───────────────────────────────────────────────

    [Header("Animator Parameter Names")]
    [Tooltip("Nombre exacto del parámetro Bool en el Animator Controller.\n" +
             "True = caminando, False = idle.")]
    [SerializeField] private string walkParam = "isWalking";

    [Tooltip("Nombre exacto del parámetro Trigger de ataque en el Animator Controller.")]
    [SerializeField] private string attackParam = "doAttack";

    [Tooltip("Nombre exacto del parámetro Trigger de muerte en el Animator Controller.")]
    [SerializeField] private string deathParam = "doDeath";

    // ─── Privados ───────────────────────────────────────────────────────────────
    private Rigidbody2D _rb;
    private Animator _animator;
    private EnemyPatrol _patrol;

    // Evita setear el bool en cada frame si no cambió
    private bool _wasWalking;

    // Evita que se procesen eventos después de morir
    private bool _isDead;

    // ───────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _patrol = GetComponent<EnemyPatrol>();
    }

    private void Update()
    {
        if (_isDead) return;

        UpdateWalkIdle();
    }

    // ─── Estados ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Detecta si el enemigo se está moviendo según la velocidad del Rigidbody2D
    /// y actualiza el parámetro Bool del Animator solo cuando hay un cambio.
    /// </summary>
    private void UpdateWalkIdle()
    {
        // Lee la dirección actual desde EnemyPatrol
        // Si la magnitud es mayor a 0 significa que está en movimiento
        bool isWalking = Mathf.Abs(_rb.linearVelocity.x) > 0.01f;

        // Solo envía el cambio al Animator si el estado cambió
        if (isWalking == _wasWalking) return;

        _animator.SetBool(walkParam, isWalking);
        _wasWalking = isWalking;
    }

    // ─── API pública ────────────────────────────────────────────────────────────

    /// <summary>
    /// Dispara la animación de ataque.
    /// Llamar desde el componente que maneje la lógica de ataque del enemigo.
    /// </summary>
    public void PlayAttack()
    {
        if (_isDead) return;
        _animator.SetTrigger(attackParam);
    }

    /// <summary>
    /// Dispara la animación de muerte y congela el resto de transiciones.
    /// Llamar desde el componente que maneje la vida o muerte del enemigo.
    /// </summary>
    public void PlayDeath()
    {
        if (_isDead) return;

        _isDead = true;
        _animator.SetBool(walkParam, false);
        _animator.SetTrigger(deathParam);
    }
}