using UnityEngine;

namespace Pentagonito
{
    /// <summary>
    /// Puerta / reja que se abre y cierra desplazándose suavemente.
    /// Llama Open(), Close() o Toggle() desde un Lever o PressurePlate.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Header("Apertura")]
        [Tooltip("Cuánto se desplaza al abrir, en unidades del mundo (ej. 0,3 sube 3).")]
        [SerializeField] private Vector2 openOffset = new Vector2(0f, 3f);
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private bool startsOpen = false;

        private Vector3 closedPos;
        private Vector3 openPos;
        private bool isOpen;

        private void Awake()
        {
            closedPos = transform.position;
            openPos = closedPos + (Vector3)openOffset;
            isOpen = startsOpen;
            transform.position = isOpen ? openPos : closedPos;
        }

        public void Open()   => isOpen = true;
        public void Close()  => isOpen = false;
        public void Toggle() => isOpen = !isOpen;

        private void Update()
        {
            Vector3 target = isOpen ? openPos : closedPos;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        }
    }
}
