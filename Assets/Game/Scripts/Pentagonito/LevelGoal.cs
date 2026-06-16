using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Pentagonito
{
    /// <summary>
    /// Meta del nivel. Al tocarla el jugador, dispara onReached y opcionalmente
    /// carga la siguiente escena. El collider debe ser Trigger.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LevelGoal : MonoBehaviour
    {
        [SerializeField] private string playerTag = "Player";

        [Tooltip("Nombre de la escena a cargar. Vacío = solo dispara el evento.")]
        [SerializeField] private string nextSceneName = "";

        public UnityEvent onReached;

        private bool reached;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (reached) return;
            if (!other.CompareTag(playerTag)) return;

            reached = true;
            onReached?.Invoke();

            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }
}
