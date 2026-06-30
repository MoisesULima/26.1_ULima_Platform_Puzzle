using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float _impulse;

    [Header("Floor Detection")]
    [SerializeField] float _sizeDetector;
    [SerializeField] LayerMask _groundMask;

    // Input Actions
    InputAction _jumpAction;
    // Player Components
    Rigidbody2D _body;
    // For floor detection
    Transform _floorDetector;
    bool _isGround;

    void Awake()
    {
        // Detectamos y guardamos el RigidBody2D del player
        _body = GetComponent<Rigidbody2D>();

        _jumpAction = InputSystem.actions["Player/Jump"];

        _floorDetector = transform.Find(Names.NAME_PLAYER_FLOOR_DETECT);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Vamos a usar el FixedUpdate para llamar 60 veces por segundo siempre, útil para cálculos físicos
    void FixedUpdate()
    {
        Collider2D floor = Physics2D.OverlapCircle(_floorDetector.position, _sizeDetector, _groundMask);
        // Si existe el piso, entonces estoy pisando (True) si no hay piso, no hay donde pisar (False)
        _isGround = floor != null;
    }

    // Update is called once per frame
    void Update()
    {
        // IsPressed se usa para mantener presionado un botón o tecla
        //if (_jumpAction.IsPressed())
        // Se tiene que cumplir las dos condiciones, que se presione el botón y el jugador esté en el piso
        if (_jumpAction.WasPressedThisFrame() && _isGround)
        {
            Debug.Log("Saltó!");
            _body.linearVelocityY = _impulse;

            AudioManager.Instance.PlaySFX(SoundEffect.Jump);
        }
    }
}
