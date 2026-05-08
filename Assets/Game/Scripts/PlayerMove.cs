using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D _body;
    // Acciones para el control
    InputAction _moveAction;

    void Awake()
    {
        // Detectamos y guardamos el RigidBody2D del player
        _body = GetComponent<Rigidbody2D>();

        _moveAction = InputSystem.actions["Player/Move"];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // El personaje va a ir a una velocidad de 5 unidades de Unity por segundo
        //_body.linearVelocityX = 5f;

        // Guardamos el movimiento, de acuerdo a lo presionado en el teclado
        Vector2 move = _moveAction.ReadValue<Vector2>();
        // Mandamos los valores de x, a la velocidad x del player
        _body.linearVelocityX = move.x * 5f;
    }
}
