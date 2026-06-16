using UnityEngine;

namespace Pentagonito
{
    /// <summary>
    /// Punto de control: actualiza el respawn del jugador al tocarlo.
    /// El collider debe ser Trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        [Tooltip("Desde dónde reaparece. Vacío = usa la posición de este objeto.")]
        [SerializeField] private Transform respawnAnchor;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            PlayerDeath death = other.GetComponentInParent<PlayerDeath>();
            if (death == null) return;

            Vector3 point = respawnAnchor != null ? respawnAnchor.position : transform.position;
            death.SetRespawnPoint(point);
        }
    }
}
