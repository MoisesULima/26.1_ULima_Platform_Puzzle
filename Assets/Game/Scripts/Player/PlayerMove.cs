using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float _speed;

    // Player Components
    Rigidbody2D _body;
    Animator _animator;
    // Acciones para el control
    InputAction _moveAction;

    void Awake()
    {
        // Detectamos y guardamos el RigidBody2D del player
        _body = GetComponent<Rigidbody2D>();
        // Detectamos y guardamos el Animator del player
        _animator = GetComponent<Animator>();

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
        _body.linearVelocityX = move.x * _speed;
        // Preguntamos si nos estamos moviendo en el eje x, y si se mueve preguntamos hacia donde apunta
        if (move.x != 0)
        {
            // Escalamos en el eje x -> 1 (mira a la derecha) -1 (mira a la izquierda)
            if (move.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        
        _animator.SetInteger("speedX", (int)move.x);
    }
}
