using UnityEngine;
using UnityEngine.InputSystem;

namespace Pentagonito
{
    /// <summary>
    /// Movimiento 2D básico con el NEW Input System (estilo project-wide actions).
    /// Lee las acciones globales del asset asignado en
    /// Project Settings > Input System Package > Project-wide Actions.
    /// Por defecto existen "Move" (Vector2) y "Jump" (Button).
    /// Detección de suelo por TAG ("Ground").
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 12f;

        [Header("Detección de suelo")]
        [Tooltip("Hijo vacío colocado a los pies del jugador.")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.15f;
        [SerializeField] private string groundTag = "Ground";

        [Header("Acciones (project-wide)")]
        [Tooltip("Nombres de las acciones globales. Deben existir en el asset project-wide.")]
        [SerializeField] private string moveActionName = "Move";
        [SerializeField] private string jumpActionName = "Jump";

        private Rigidbody2D rb;
        private InputAction moveAction;
        private InputAction jumpAction;

        private float horizontalInput;
        private bool isGrounded;
        private bool jumpRequested;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true; // que el jugador no gire al chocar

            // InputSystem.actions = el asset project-wide asignado en Project Settings.
            var actions = InputSystem.actions;
            if (actions == null)
            {
                Debug.LogError("[Pentagonito] No hay un asset project-wide asignado en " +
                               "Project Settings > Input System Package.");
                return;
            }

            moveAction = actions.FindAction(moveActionName);
            jumpAction = actions.FindAction(jumpActionName);
        }

        private void OnEnable()
        {
            // Las acciones project-wide ya vienen habilitadas, pero lo aseguramos.
            // No las deshabilitamos en OnDisable porque son compartidas por todos.
            moveAction?.Enable();
            jumpAction?.Enable();
        }

        private void Update()
        {
            // "Move" es Vector2; tomamos solo el eje X.
            horizontalInput = moveAction != null ? moveAction.ReadValue<Vector2>().x : 0f;

            if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
                jumpRequested = true;
        }

        private void FixedUpdate()
        {
            CheckGround();

            // En Unity 6 la propiedad es linearVelocity.
            // (En Unity 2022 o anterior usa rb.velocity en su lugar.)
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

            if (jumpRequested)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpRequested = false;
            }
        }

        private void CheckGround()
        {
            isGrounded = false;
            if (groundCheck == null) return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag(groundTag))
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
