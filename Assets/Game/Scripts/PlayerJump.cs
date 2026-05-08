using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] float _impulse;

    InputAction _jumpAction;
    Rigidbody2D _body;
    void Awake()
    {
        // Detectamos y guardamos el RigidBody2D del player
        _body = GetComponent<Rigidbody2D>();

        _jumpAction = InputSystem.actions["Player/Jump"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // IsPressed se usa para mantener presionado un botón o tecla
        //if (_jumpAction.IsPressed())
        if (_jumpAction.WasPressedThisFrame())
        {
            Debug.Log("Saltó!");
            _body.linearVelocityY = _impulse;
        }
    }
}
