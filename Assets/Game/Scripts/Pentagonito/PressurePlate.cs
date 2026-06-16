using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Pentagonito
{
    /// <summary>
    /// Placa de presión: se activa mientras algo válido esté encima
    /// y se libera cuando ya nada la pisa.
    /// Puede ser un botón honesto (onPressed -> Door.Open) o una trampa
    /// (onReleased -> Trap.Spring: "no te bajes de la placa o caen pinchos").
    /// El collider debe ser Trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PressurePlate : MonoBehaviour
    {
        [Header("Qué puede activarla (por tag)")]
        [SerializeField] private string[] validTags = { "Player", "Box" };

        [Header("Eventos")]
        public UnityEvent onPressed;   // primera cosa que pisa
        public UnityEvent onReleased;  // ya no queda nada encima

        private readonly HashSet<Collider2D> occupants = new HashSet<Collider2D>();

        private bool IsValid(Collider2D c)
        {
            foreach (var t in validTags)
                if (c.CompareTag(t)) return true;
            return false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsValid(other)) return;

            bool wasEmpty = occupants.Count == 0;
            occupants.Add(other);
            if (wasEmpty) onPressed?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!occupants.Remove(other)) return;
            if (occupants.Count == 0) onReleased?.Invoke();
        }
    }
}
