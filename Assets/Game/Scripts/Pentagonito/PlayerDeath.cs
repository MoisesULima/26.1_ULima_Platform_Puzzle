using System.Collections;
using UnityEngine;

namespace Pentagonito
{
    /// <summary>
    /// Maneja la muerte y el respawn del jugador.
    /// Los peligros (Hazard) llaman a Kill(); los Checkpoint llaman a SetRespawnPoint().
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerDeath : MonoBehaviour
    {
        [Header("Respawn")]
        [Tooltip("Pausa breve antes de reaparecer (efecto de muerte).")]
        [SerializeField] private float respawnDelay = 0.35f;

        private Vector3 respawnPoint;
        private Rigidbody2D rb;
        private bool isDead;

        public bool IsDead => isDead;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            respawnPoint = transform.position; // punto de partida = primer respawn
        }

        public void SetRespawnPoint(Vector3 point) => respawnPoint = point;

        public void Kill()
        {
            if (isDead) return;
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            isDead = true;

            // Congelamos al jugador un instante (aquí puedes meter animación/sonido).
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;

            yield return new WaitForSeconds(respawnDelay);

            // Por si una plataforma móvil lo dejó emparentado, lo soltamos.
            transform.SetParent(null);
            transform.position = respawnPoint;

            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            isDead = false;
        }
    }
}
