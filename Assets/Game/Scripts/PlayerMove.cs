using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D _body;

    void Awake()
    {
        // Detectamos y guardamos el RigidBody2D del player
        _body = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // El personaje va a ir a una velocidad de 5 unidades de Unity por segundo
        _body.linearVelocityX = 5f;
    }
}
