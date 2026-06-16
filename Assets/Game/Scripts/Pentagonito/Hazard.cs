using UnityEngine;

namespace Pentagonito
{
    /// <summary>
    /// Cualquier objeto mortal: pinchos, sierras, trampas activables.
    /// Detecta al jugador por TAG ("Player") y lo mata.
    /// Puede empezar desactivado (isActive = false) y ser encendido por un Trap.
    /// Funciona tanto si el collider es Trigger como si es sólido.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        [Tooltip("Si está activo, mata al contacto. Un Trap puede encenderlo en runtime.")]
        [SerializeField] private bool isActive = true;

        public void SetActive(bool value) => isActive = value;

        // Trigger (collider marcado como Is Trigger)
        private void OnTriggerEnter2D(Collider2D other) => TryKill(other);
        private void OnTriggerStay2D(Collider2D other)  => TryKill(other); // mata aunque se encienda encima del jugador

        // Collider sólido
        private void OnCollisionEnter2D(Collision2D c) => TryKill(c.collider);
        private void OnCollisionStay2D(Collision2D c)  => TryKill(c.collider);

        private void TryKill(Collider2D other)
        {
            if (!isActive) return;
            if (!other.CompareTag(playerTag)) return;

            // El componente de muerte puede estar en el objeto o en su raíz.
            PlayerDeath death = other.GetComponentInParent<PlayerDeath>();
            if (death != null)
                death.Kill();
        }
    }
}
