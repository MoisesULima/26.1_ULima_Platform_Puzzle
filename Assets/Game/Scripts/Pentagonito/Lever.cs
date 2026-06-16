using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Pentagonito
{
    /// <summary>
    /// Palanca accionada con el NEW Input System (estilo project-wide actions).
    /// Usa la acción global "Interact" (Button) por defecto.
    /// El collider de este objeto debe ser Trigger (la "zona de alcance").
    /// Conecta onActivate / onDeactivate EN EL INSPECTOR:
    ///   - a Door.Open()   -> palanca honesta
    ///   - a Trap.Spring() -> palanca trampa (se ve igual que la honesta)
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Lever : MonoBehaviour
    {
        [Header("Configuración")]
        [SerializeField] private string playerTag = "Player";
        [Tooltip("Acción global del asset project-wide (por defecto 'Interact').")]
        [SerializeField] private string interactActionName = "Interact";
        [SerializeField] private bool startsOn = false;
        [Tooltip("Si es true, solo se puede accionar una vez.")]
        [SerializeField] private bool oneShot = false;

        [Header("Eventos")]
        public UnityEvent onActivate;
        public UnityEvent onDeactivate;

        private InputAction interactAction;
        private bool playerInRange;
        private bool isOn;
        private bool used;

        private void Awake()
        {
            var actions = InputSystem.actions;
            if (actions == null)
            {
                Debug.LogError("[Pentagonito] No hay un asset project-wide asignado en " +
                               "Project Settings > Input System Package.");
                return;
            }
            interactAction = actions.FindAction(interactActionName);
        }

        private void OnEnable() => interactAction?.Enable();

        private void Start() => isOn = startsOn;

        private void Update()
        {
            if (!playerInRange) return;
            if (oneShot && used) return;

            if (interactAction != null && interactAction.WasPressedThisFrame())
                Toggle();
        }

        public void Toggle()
        {
            isOn = !isOn;
            used = true;
            if (isOn) onActivate?.Invoke();
            else      onDeactivate?.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag)) playerInRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(playerTag)) playerInRange = false;
        }
    }
}
