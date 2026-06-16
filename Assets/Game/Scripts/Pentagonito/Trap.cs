using UnityEngine;
using UnityEngine.Events;

namespace Pentagonito
{
    /// <summary>
    /// Trampa: lo que un botón/palanca "malo" dispara en lugar de abrir una puerta.
    /// Puede encender pinchos, activar/desactivar objetos o soltar un bloque que cae.
    /// Conecta un Lever.onActivate o PressurePlate.onReleased a Spring().
    /// </summary>
    public class Trap : MonoBehaviour
    {
        [Header("Pinchos a encender (opcional)")]
        [SerializeField] private Hazard[] hazardsToEnable;

        [Header("Objetos a activar/desactivar (opcional)")]
        [SerializeField] private GameObject[] objectsToEnable;
        [SerializeField] private GameObject[] objectsToDisable;

        [Header("Cuerpo que cae (opcional)")]
        [Tooltip("Rigidbody2D que estaba Kinematic y empieza a caer al dispararse.")]
        [SerializeField] private Rigidbody2D fallingBody;

        [Header("Evento extra")]
        public UnityEvent onSprung;

        private bool sprung;

        public void Spring()
        {
            if (sprung) return;
            sprung = true;

            if (hazardsToEnable != null)
                foreach (var h in hazardsToEnable)
                    if (h != null) h.SetActive(true);

            if (objectsToEnable != null)
                foreach (var go in objectsToEnable)
                    if (go != null) go.SetActive(true);

            if (objectsToDisable != null)
                foreach (var go in objectsToDisable)
                    if (go != null) go.SetActive(false);

            if (fallingBody != null)
                fallingBody.bodyType = RigidbodyType2D.Dynamic;

            onSprung?.Invoke();
        }

        /// Permite re-armar la trampa (no se llama "Reset" para no chocar con el menú del editor).
        public void Rearm() => sprung = false;
    }
}
