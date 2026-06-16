using UnityEngine;

namespace Pentagonito
{
    /// <summary>
    /// Plataforma que se mueve entre dos puntos (pointA y pointB).
    /// Carga al jugador emparentándolo mientras está encima.
    /// Puede ser camino seguro... o llevarte directo a los pinchos.
    /// IMPORTANTE: mantén la escala de la plataforma en (1,1,1) para no deformar al jugador.
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float speed = 2f;
        [SerializeField] private string playerTag = "Player";

        [Tooltip("Si es true arranca sola; si no, espera a StartMoving().")]
        [SerializeField] private bool autoStart = true;

        private bool moving;
        private Vector3 target;

        private void Start()
        {
            target = pointB != null ? pointB.position : transform.position;
            moving = autoStart;
        }

        public void StartMoving() => moving = true;
        public void StopMoving()  => moving = false;

        private void Update()
        {
            if (!moving || pointA == null || pointB == null) return;

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.01f)
                target = (target == pointA.position) ? pointB.position : pointA.position;
        }

        // Emparenta al jugador para que viaje con la plataforma.
        private void OnCollisionEnter2D(Collision2D c)
        {
            if (c.collider.CompareTag(playerTag))
                c.collider.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D c)
        {
            if (c.collider.CompareTag(playerTag))
                c.collider.transform.SetParent(null);
        }
    }
}
